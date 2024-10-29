using StpFoodBlazor.Models;
using System;

namespace StpFoodBlazor.Helpers {
    public class DealFilter {
        public DealEvent[]? Deals { get; set; }
        public string? Day { get; set; }
        public string? Name { get; set; }
        public bool? Alcohol { get; set; }
        public bool? HappyHour { get; set; }

        public DealEvent[] filter() {
            if (Deals == null) {
                return new DealEvent[0];
            }

            DealEvent[] filteredDeals = Deals;

            if (!string.IsNullOrWhiteSpace(Day)) {
                filteredDeals = filterByDay(filteredDeals, Day);
            }

            if (!string.IsNullOrWhiteSpace(Name)) {
                filteredDeals = filterByName(filteredDeals, Name);
            }

            if (Alcohol != null) {
                filteredDeals = filterByAlcohol(filteredDeals, (bool)Alcohol);
            }

            if(HappyHour != null) {
                filteredDeals = filterByHappyHour(filteredDeals, (bool)HappyHour);
            }
            // if (Name != null) {
            //     filteredDeals = filterByName(filteredDeals);
            // }
            // if (Alcohol != null) {
            //     filteredDeals = filterByAlcohol(filteredDeals);
            // }
            // if (HappyHour != null) {
            //     filteredDeals = filterByHappyHour(filteredDeals);
            // }
            return filteredDeals;
        }

        private static DealEvent[] filterByDay(DealEvent[] deals, String day) {
            return deals.Where(deal => deal.Day == day).ToArray();
        }

        private static DealEvent[] filterByName(DealEvent[] deals, String name) {
            return deals.Where(deal => deal.Name == name).ToArray();
        }

        private static DealEvent[] filterByAlcohol(DealEvent[] deals, Boolean alcohol) {
            if (alcohol) {
                return deals.Where(deal => !string.IsNullOrWhiteSpace(deal.Alcohol)).ToArray();
            }

            return deals.Where(deal => string.IsNullOrWhiteSpace(deal.Alcohol)).ToArray();
        }

        // Need to convert column header with space to model attribute without one
        private static DealEvent[] filterByHappyHour(DealEvent[] deals, Boolean happyHour) {
            if (happyHour) {
                return deals.Where(deal =>
                    deal.HappyHour != null && deal.HappyHour.Trim() != "").ToArray();
            }

            return deals.Where(deal =>
                    deal.HappyHour == null || deal.HappyHour.Trim() == "").ToArray();
        }
    }
}
