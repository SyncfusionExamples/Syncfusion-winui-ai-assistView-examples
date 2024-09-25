using Syncfusion.UI.Xaml.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingStarted
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<object> chats;
        private Author currentUser;
        public ViewModel()
        {
            this.Chats = new ObservableCollection<object>();
            this.CurrentUser = new Author { Name = "John" };
            this.GenerateMessages();
        }

        private async void GenerateMessages()
        {
            this.Chats.Add(new TextMessage { Author = CurrentUser, Text = "What is WinUI?" });
            await Task.Delay(1000);
            this.Chats.Add(new TextMessage { Author = new Author { Name = "Bot" }, Text = "WinUI is a user interface layer that contains modern controls and styles for building Windows apps." });
        }

        public ObservableCollection<object> Chats
        {
            get
            {
                return this.chats;
            }
            set
            {
                this.chats = value;
                RaisePropertyChanged("Messages");
            }
        }

        public Author CurrentUser
        {
            get
            {
                return this.currentUser;
            }
            set
            {
                this.currentUser = value;
                RaisePropertyChanged("CurrentUser");
            }
        }


        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
