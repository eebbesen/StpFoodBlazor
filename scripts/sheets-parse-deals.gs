const CONFIG = {
  CLAUDE_MODEL: 'claude-sonnet-4-6',
  GMAIL_PROCESS_LABEL: 'deals-to-process',
  GMAIL_DONE_LABEL: 'deals-processed',
  INBOX_SHEET: 'Inbox',
  EXAMPLES_SHEET: 'Examples',
};

// ── Entry point ──────────────────────────────────────────────────────────────

function processNewEmails() {
  const apiKey = PropertiesService.getScriptProperties().getProperty('CLAUDE_API_KEY');
  const ss = SpreadsheetApp.getActiveSpreadsheet();
  const inboxSheet = ss.getSheetByName(CONFIG.INBOX_SHEET);
  const systemPrompt = buildSystemPrompt(ss);
  const doneLabel = GmailApp.getUserLabelByName(CONFIG.GMAIL_DONE_LABEL)
    || GmailApp.createLabel(CONFIG.GMAIL_DONE_LABEL);

  const threads = GmailApp.search(
    `label:${CONFIG.GMAIL_PROCESS_LABEL} -label:${CONFIG.GMAIL_DONE_LABEL}`
  );

  for (const thread of threads) {
    for (const message of thread.getMessages()) {
      try {
        const deals = extractDeals(message, systemPrompt, apiKey);
        if (deals.length > 0) writeToInbox(inboxSheet, deals, message);
      } catch (e) {
        Logger.log(`Error on message ${message.getId()}: ${e.message}`);
      }
    }
    thread.addLabel(doneLabel);
  }
}

// ── Extraction ───────────────────────────────────────────────────────────────

function extractDeals(message, systemPrompt, apiKey) {
  const content = buildMessageContent(message);
  const response = callClaude(systemPrompt, content, apiKey);
  return parseDeals(response);
}

function buildMessageContent(message) {
  const textParts = [
    `Subject: ${message.getSubject()}`,
    `From: ${message.getFrom()}`,
    `Body:\n${message.getPlainBody()}`,
  ];

  const contentBlocks = [{ type: 'text', text: textParts.join('\n\n') }];

  for (const att of message.getAttachments()) {
    if (att.getContentType().startsWith('image/')) {
      contentBlocks.push({
        type: 'image',
        source: {
          type: 'base64',
          media_type: att.getContentType(),
          data: Utilities.base64Encode(att.getBytes()),
        },
      });
    }
  }

  return contentBlocks;
}

// ── Claude API ───────────────────────────────────────────────────────────────

function callClaude(systemPrompt, contentBlocks, apiKey) {
  const res = UrlFetchApp.fetch('https://api.anthropic.com/v1/messages', {
    method: 'POST',
    muteHttpExceptions: true,
    headers: {
      'x-api-key': apiKey,
      'anthropic-version': '2023-06-01',
      'content-type': 'application/json',
    },
    payload: JSON.stringify({
      model: CONFIG.CLAUDE_MODEL,
      max_tokens: 2048,
      system: systemPrompt,
      messages: [{ role: 'user', content: contentBlocks }],
    }),
  });

  if (res.getResponseCode() !== 200) {
    throw new Error(`Claude API ${res.getResponseCode()}: ${res.getContentText()}`);
  }

  return JSON.parse(res.getContentText());
}

function parseDeals(apiResponse) {
  const text = apiResponse.content[0].text.trim();
  try {
    const parsed = JSON.parse(text);
    return Array.isArray(parsed) ? parsed : [];
  } catch (_) {
    const match = text.match(/\[[\s\S]*\]/);
    if (match) {
      try { return JSON.parse(match[0]); } catch (_) {}
    }
    Logger.log(`Unparseable response: ${text}`);
    return [];
  }
}

// ── Prompt ───────────────────────────────────────────────────────────────────

function buildSystemPrompt(ss) {
  const examples = getExamples(ss.getSheetByName(CONFIG.EXAMPLES_SHEET));
  const examplesSection = examples.length > 0
    ? `\n\nExamples of correctly extracted deals:\n${JSON.stringify(examples, null, 2)}`
    : '';

  return `You extract food and drink deal information from restaurant marketing emails and return structured JSON.

Extract ALL deals mentioned. Return a JSON array — one object per deal. Use null for any field not present in the email.

Fields:
- name: Restaurant or bar name
- deal: Concise deal description
- day: Day of week ("Sunday"–"Saturday"), or null if the deal runs all week or the day is unknown  
- happyHour: Time range as "HH:MM - HH:MM" (24h), or null
- start: Start date as "YYYY-MM-DD" for limited-time deals, or null
- end: End date as "YYYY-MM-DD" if the deal expires, or null
- alcohol: "alcohol" if the deal includes drinks, null otherwise
- url: URL for the deal if present, null otherwise

Rules:
- If the email contains no deal information, return []
- Do not invent or infer information not stated in the email
- Return ONLY the JSON array, no explanation or markdown${examplesSection}`;
}

function getExamples(sheet) {
  if (!sheet) return [];
  const rows = sheet.getDataRange().getValues().slice(1); // skip header
  return rows
    .filter(r => r[0])
    .map(r => ({
      name:       r[0] || null,
      deal:       r[1] || null,
      day:        r[2] || null,
      happyHour:  r[3] || null,
      start:      r[4] || null,
      end:        r[5] || null,
      alcohol:    r[6] || null,
      url:        r[7] || null,
    }));
}

// ── Sheet write ──────────────────────────────────────────────────────────────

function writeToInbox(sheet, deals, message) {
  const now = new Date();
  for (const d of deals) {
    sheet.appendRow([
      d.name, d.deal, d.day, d.happyHour, d.start, d.end, d.alcohol, d.url,
      message.getId(), message.getSubject(), message.getDate(), now,
      'pending', '',
    ]);
  }
}

// ── One-time setup ───────────────────────────────────────────────────────────

function setup() {
  const ss = SpreadsheetApp.getActiveSpreadsheet();

  if (!ss.getSheetByName(CONFIG.INBOX_SHEET)) {
    const s = ss.insertSheet(CONFIG.INBOX_SHEET);
    s.appendRow([
      'Name', 'Deal', 'Day', 'Happy Hour', 'Start', 'End', 'Alcohol', 'URL',
      'Source Email ID', 'Source Subject', 'Source Date', 'Processed Date',
      'Review Status', 'Review Notes',
    ]);
    s.setFrozenRows(1);
  }

  if (!ss.getSheetByName(CONFIG.EXAMPLES_SHEET)) {
    const s = ss.insertSheet(CONFIG.EXAMPLES_SHEET);
    s.appendRow(['Name', 'Deal', 'Day', 'Happy Hour', 'Start', 'End', 'Alcohol', 'URL', 'Notes']);
    s.setFrozenRows(1);
  }

  if (!GmailApp.getUserLabelByName(CONFIG.GMAIL_PROCESS_LABEL))
    GmailApp.createLabel(CONFIG.GMAIL_PROCESS_LABEL);
  if (!GmailApp.getUserLabelByName(CONFIG.GMAIL_DONE_LABEL))
    GmailApp.createLabel(CONFIG.GMAIL_DONE_LABEL);

  Logger.log('Setup complete.');
}
