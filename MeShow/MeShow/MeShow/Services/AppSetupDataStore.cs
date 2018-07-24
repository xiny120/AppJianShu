using MeShow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(MeShow.Services.AppSetupDataStore))]

namespace MeShow.Services
{

    public class AppSetupDataStore : IDataStore<AppSetup>
    {
        List<AppSetup> items;

        public AppSetupDataStore()
        {
            items = new List<AppSetup>();
            var mockItems = new List<AppSetup>
            {
                new AppSetup {Id = Guid.NewGuid().ToString() ,
                    tabItems = new TabItem[] {
                        new TabItem { Id = Guid.NewGuid().ToString(), Text = "米秀", ImageSource = "icon.png" },
                        new TabItem { Id = Guid.NewGuid().ToString(), Text = "关注", ImageSource = "follow.png" },
                        new TabItem { Id = Guid.NewGuid().ToString(), Text = "我的", ImageSource = "my.png" },
                    } },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." },
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(AppSetup item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(AppSetup item)
        {
            var oldItem = items.Where((AppSetup arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((AppSetup arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<AppSetup> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<AppSetup>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}
