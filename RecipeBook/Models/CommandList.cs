namespace RecipeBook.Models
{
    delegate bool CommandAction(params String[] args);
    class Command(string name, CommandAction callback, string trigger)
    {
        public string Trigger { get; set; } = trigger;
        public string Name { get; set; } = name;
        public CommandAction CallBack { get; set; } = callback;
    }
    class CommandList : List<Command>
    {
        public void Add(string name, CommandAction callback, string? trigger = null)
        {
            base.Add(new Command(name, callback, getValidTrigger(trigger, name)));
        }

        public bool Run(string? commandTrigger, params String[] args)
        {
            var command = this.FirstOrDefault(cmd => cmd.Trigger == commandTrigger || cmd.Name == commandTrigger);
            if (command != null && commandTrigger != null)
            {
                return command.CallBack.Invoke(args);
            }
            else
            {
                return InvalidChoice();
            }
        }

        public static bool InvalidChoice(string? message = null)
        {
            if (message != null)
            {
                if (!message.StartsWith(' '))
                {
                    message = " " + message;
                }
                if (!message.EndsWith('.'))
                {
                    message += ".";
                }
            }
            else
            {
                message = "";
            }
            Console.WriteLine($"Invalid choice.{message} Press Enter to try again.");
            Console.ReadLine();
            return true;
        }

        private string getValidTrigger(string? trigger, string name)
        {
            var proposedTrigger = trigger;
            if (String.IsNullOrWhiteSpace(proposedTrigger))
            {
                proposedTrigger = (this.Count + 1).ToString();
            }
            for (int i = 1; i < name.Length; i++)
            {
                if (!this.Any(cmd => cmd.Trigger == proposedTrigger))
                {
                    break;
                }
                proposedTrigger = name.Substring(0, i).ToLower();
            }
            if (this.Any(cmd => cmd.Trigger == proposedTrigger))
            {
                throw new ArgumentException($"We were unable to find a unique trigger for the '{name}' command.");
            }
            return proposedTrigger;
        }
    }
}
