using System.Net;

namespace Farm.Api.Models
{
    public class NavigationItem
    {
        public string DisplayName { get; set; }
        public string Route { get; set; }
        public string Icon { get; set; }
        public string[] Roles { get; set; }

        public bool IsAuthorized { get; set; }

        public string Url { get; set; }

        public IList<NavigationItem> Children { get; set; }

        /// <summary>
        /// Get all navigation items
        /// </summary>
        /// <returns>List of NavigationItem</returns>
        public static IList<NavigationItem> GetAll()
        {
            var items = new List<NavigationItem>
            {
                new NavigationItem
                {
                    DisplayName = "Reports",
                    Route = "app/reports",
                    Icon = "dashboard",
                    IsAuthorized = true,
                },
                new NavigationItem
                {
                    DisplayName = "Farms",
                    Route = "app/farms",
                    Icon = "agriculture",
                    IsAuthorized = true,
                },
                new NavigationItem
                {
                    DisplayName = "Cages",
                    Route = "app/cages",
                    Icon = "home",
                    IsAuthorized = true,
                },
                new NavigationItem
                {
                    DisplayName = "Animals",
                    Route = "app/animals",
                    Icon = "pets",
                    IsAuthorized = true,
                },

                // ===== Phase 1 =====
                new NavigationItem
                {
                    DisplayName = "Vaccines",
                    Route = "app/vaccines",
                    Icon = "vaccines",
                    IsAuthorized = true,
                },
                new NavigationItem
                {
                    DisplayName = "Feeds",
                    Route = "app/feeds",
                    Icon = "restaurant",
                    IsAuthorized = true,
                },
                new NavigationItem
                {
                    DisplayName = "Growth Logs",
                    Route = "app/growth-logs",
                    Icon = "monitor_weight",
                    IsAuthorized = true,
                },
                new NavigationItem
                {
                    DisplayName = "Diseases",
                    Route = "app/diseases",
                    Icon = "healing",
                    IsAuthorized = true,
                },
                new NavigationItem
                {
                    DisplayName = "Alerts",
                    Route = "app/alerts",
                    Icon = "notifications",
                    IsAuthorized = true,
                },

                new NavigationItem
                {
                    DisplayName = "Users",
                    Route = "app/users",
                    Icon = "people",
                    IsAuthorized = true,
                }
            };
            return items;
        }
    }
}
