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
        public static CommandAction GoBackAction = static (_) => { return true; };
        public static CommandAction DeepGoBackAction = static (_) => { return false; };
        public void Add(string name, CommandAction callback, string? trigger = null)
        {
            base.Add(new Command(name, callback, getValidTrigger(trigger, name)));
        }

        public bool Run(string? commandTrigger, params String[] args)
        {
            var command = this.FirstOrDefault(cmd => cmd.Trigger.Equals(commandTrigger, StringComparison.OrdinalIgnoreCase) || cmd.Name.Equals(commandTrigger, StringComparison.OrdinalIgnoreCase));
            if (command != null && commandTrigger != null)
            {
                return command.CallBack.Invoke(args);
            }
            else
            {
                return InvalidChoice();
            }
        }

        public static bool ValidateArgs(string[] args, int required, string? errorMessage = "You need to provide all required parameters")
        {
            if (args.Length < required)
            {
                return !InvalidChoice(errorMessage);
            }
            return true;
        }

        public static bool InvalidChoice(string? message = null, string? errorMessage = null)
        {
            if (message is not null)
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
            if (errorMessage is not null)
            {
                if (!message.EndsWith('.'))
                {
                    message += ".";
                }
                Console.WriteLine($"Error: {errorMessage}");
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
                if (!this.Any(cmd => string.Equals(cmd.Trigger, proposedTrigger, StringComparison.OrdinalIgnoreCase)))
                {
                    break;
                }
                proposedTrigger = name.Substring(0, i).ToLower();
            }
            if (this.Any(cmd => string.Equals(cmd.Trigger, proposedTrigger, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"We were unable to find a unique trigger for the '{name}' command.");
            }
            return proposedTrigger;
        }
    }
}
