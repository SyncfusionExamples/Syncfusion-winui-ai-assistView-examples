using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Syncfusion.UI.Xaml.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Microsoft.VisualBasic;
using System.Net;

namespace OpenAI_Sample
{
    public class ViewModel : INotifyPropertyChanged
    {
        public AIAssistChatService service;

        private ObservableCollection<object> chats;
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
        public DataTemplate AIIcon { get; set; }
        private ObservableCollection<string> suggestion;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Author currentUser;
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

        private bool showTypingIndicator;
        public bool ShowTypingIndicator
        {
            get
            {
                return this.showTypingIndicator;
            }
            set
            {
                this.showTypingIndicator = value;
                RaisePropertyChanged("ShowTypingIndicator");
            }
        }

        public ObservableCollection<string> Suggestion
        {
            get
            {
                return this.suggestion;
            }
            set
            {
                this.suggestion = value;
                RaisePropertyChanged("Suggestion");
            }
        }

        private TypingIndicator typingIndicator;
        public TypingIndicator TypingIndicator
        {
            get
            {
                return this.typingIndicator;
            }
            set
            {
                this.typingIndicator = value;
                RaisePropertyChanged("TypingIndicator");
            }
        }

        public ViewModel()
        {
            this.Chats = new ObservableCollection<object>();
            this.Chats.CollectionChanged += Chats_CollectionChanged;
            this.CurrentUser = new Author() { Name = "User" };
            this.TypingIndicator = new TypingIndicator() { Author = new Author { ContentTemplate = AIIcon } };
            service = new AIAssistChatService();
            //service.Initialize();
            
        }

        private async void Chats_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var item = e.NewItems?[0] as ITextMessage;
            if (item != null)
            {
                if (item.Author.Name == currentUser.Name && service.isOpenAIConnect)
                {
                    ShowTypingIndicator = true;
                    await service.NonStreamingChat(item.Text);
                    Chats.Add(new TextMessage
                    {
                        Author = new Author { Name = "Bot", ContentTemplate = AIIcon },
                        DateTime = DateTime.Now,
                        Text = service.Response
                    });
                    ShowTypingIndicator = false;
                }
                else if (item.Text != "Please enter the Model Name, Endpoint and Key to initialize the service." && !service.isOpenAIConnect)
                {
                    ShowTypingIndicator = true;
                    await Task.Delay(1000);
                    Chats.Add(new TextMessage
                    {
                        Author = new Author { Name = "Bot", ContentTemplate = AIIcon },
                        DateTime = DateTime.Now,
                        Text = "Please enter the Model Name, Endpoint and Key to initialize the service."
                    });
                    ShowTypingIndicator = false;
                }
            }
        }

        public class AIAssistChatService
        {
            IChatCompletionService gpt;
            Kernel kernel;
            internal bool isOpenAIConnect;

            public string Response { get; set; }
            public async Task Initialize( string ModelName,string Endpoint,string Key)
            {
                if(ModelName!= null && Endpoint != null && Key != null)
                {
                    var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(
                 ModelName, Endpoint, Key);
                    kernel = builder.Build();
                    gpt = kernel.GetRequiredService<IChatCompletionService>();
                    isOpenAIConnect = true;
                }
                else
                {
                    isOpenAIConnect = false;
                }
               
            }
            public async Task NonStreamingChat(string line)
            {
                Response = string.Empty;
                var response = await gpt.GetChatMessageContentAsync(line);
                Response = response.ToString();
            }
        }
    }
}
