// Paste this into your Google Sheet's Apps Script editor (Extensions > Apps Script).
// Set the constants below, then add an installable onChange trigger:
//   Triggers > Add Trigger > onSheetChange | From spreadsheet | On change
//
// TAB_KEYS maps each tab name (lowercase) to the cache key it invalidates.
// Changes to any other tab are ignored.
//
// Main sheet:     { "deals": "deals", "giftcards": "giftcards" }
// Snelling sheet: { "deals": "deals_snelrand" }

const APP_URLS = [
  "https://your-app-1.azurewebsites.net/api/cache/invalidate",
  "https://your-app-2.azurewebsites.net/api/cache/invalidate"
];
const CACHE_INVALIDATION_KEY = "your-secret-key-here";
const TAB_KEYS = { "deals": "deals", "giftcards": "giftcards" };

function onSheetChange(e) {
  var sheetName = e.source.getActiveSheet().getName().toLowerCase();
  var cacheKey = TAB_KEYS[sheetName];

  if (!cacheKey) {
    Logger.log("Ignoring change on sheet: " + sheetName);
    return;
  }

  APP_URLS.forEach(function(appUrl) {
    try {
      var response = UrlFetchApp.fetch(appUrl + "?key=" + cacheKey, {
        method: "post",
        headers: {
          "X-Cache-Invalidation-Key": CACHE_INVALIDATION_KEY
        },
        muteHttpExceptions: true
      });

      if (response.getResponseCode() === 200) {
        Logger.log("Cache invalidated for: " + cacheKey + " at " + appUrl);
      } else {
        Logger.log("Cache invalidation failed for " + appUrl + ": " + response.getResponseCode() + " " + response.getContentText());
      }
    } catch (err) {
      Logger.log("Cache invalidation error for " + appUrl + ": " + err.message);
    }
  });
}
