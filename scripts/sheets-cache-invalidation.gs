// Paste this into your Google Sheet's Apps Script editor (Extensions > Apps Script).
// Set the constants below, then add an installable onChange trigger:
//   Triggers > Add Trigger > onSheetChange | From spreadsheet | On change
//
// Sheet tab names must match the cache keys: "deals" and "giftcards".
// Changes to any other tab are ignored.

const APP_URLS = [
  "https://your-app-1.azurewebsites.net/api/cache/invalidate",
  "https://your-app-2.azurewebsites.net/api/cache/invalidate"
];
const CACHE_INVALIDATION_KEY = "your-secret-key-here";
const VALID_KEYS = ["deals", "giftcards"];

function onSheetChange(e) {
  var sheetName = e.source.getActiveSheet().getName().toLowerCase();

  if (!VALID_KEYS.includes(sheetName)) {
    Logger.log("Ignoring change on sheet: " + sheetName);
    return;
  }

  APP_URLS.forEach(function(appUrl) {
    try {
      var response = UrlFetchApp.fetch(appUrl + "?key=" + sheetName, {
        method: "post",
        headers: {
          "X-Cache-Invalidation-Key": CACHE_INVALIDATION_KEY
        },
        muteHttpExceptions: true
      });

      if (response.getResponseCode() === 200) {
        Logger.log("Cache invalidated for: " + sheetName + " at " + appUrl);
      } else {
        Logger.log("Cache invalidation failed for " + appUrl + ": " + response.getResponseCode() + " " + response.getContentText());
      }
    } catch (err) {
      Logger.log("Cache invalidation error for " + appUrl + ": " + err.message);
    }
  });
}
