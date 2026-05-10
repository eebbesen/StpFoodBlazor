// Paste this into your Google Sheet's Apps Script editor (Extensions > Apps Script).
// Set the two constants below, then add an installable onChange trigger:
//   Triggers > Add Trigger > onSheetChange | From spreadsheet | On change
//
// Sheet tab names must match the cache keys: "deals" and "giftcards".
// Changes to any other tab are ignored.

const APP_URL = "https://your-app.azurewebsites.net/api/cache/invalidate";
const CACHE_INVALIDATION_KEY = "your-secret-key-here";
const VALID_KEYS = ["deals", "giftcards"];

function onSheetChange(e) {
  var sheetName = e.source.getActiveSheet().getName().toLowerCase();

  if (!VALID_KEYS.includes(sheetName)) {
    Logger.log("Ignoring change on sheet: " + sheetName);
    return;
  }

  try {
    var response = UrlFetchApp.fetch(APP_URL + "?key=" + sheetName, {
      method: "post",
      headers: {
        "X-Cache-Invalidation-Key": CACHE_INVALIDATION_KEY
      },
      muteHttpExceptions: true
    });

    if (response.getResponseCode() === 200) {
      Logger.log("Cache invalidated for: " + sheetName);
    } else {
      Logger.log("Cache invalidation failed: " + response.getResponseCode() + " " + response.getContentText());
    }
  } catch (err) {
    Logger.log("Cache invalidation error: " + err.message);
  }
}
