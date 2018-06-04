using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ConsoleExtensions;

namespace Commands
{
    public abstract class Command : ICommand
    {
        public abstract string CommandName { get; }
        public abstract string Description { get; }
        
        protected IList<CommandArgument> Schemas { get; set; }
        protected IList<string> Arguments { get; set; }

        protected Command()
        {
            Schemas = new List<CommandArgument>
            {
                new CommandArgument
                {
                    Name = "help",
                    ShortName = "h",
                    Description = "Shows detail of the command."
                }
            };
            Arguments = new List<string>();
        }
        
        protected abstract void OnExecute();

        protected abstract ICommand Filter();

        public void Execute()
        {
            var isHelp = HasArgument("help");
            if (isHelp)
            {
                Help();
            }
            else
            {
                OnExecute();
            }
        }

        protected void AddArgument(string name, string shortName = "", string description = "", bool isReqruied = false, bool isUnary = false, string format = null)
        {
            if (Schemas.Any(c => c.Name.Equals(name)))
            {
                throw new ArgumentException($"Argument {name} is already defined.");
            }
            Schemas.Add(new CommandArgument
            {
                Name = name,
                ShortName = shortName,
                Description = description,
                IsRequired = isReqruied,
                IsUninary = isUnary,
                Format = format
            });
        }

        public virtual ICommand ReadArguments(IEnumerable<string> args)
        {
            Arguments = args.ToList();
            if (HasArgument("help"))
            {
                return this;
            }
            Validate();
            Filter();
            return this;
        }

        private void Validate()
        {
            "Verifying arguments...".PrettyPrint(ConsoleColor.White);
            foreach (var schema in Schemas)
            {
                if (schema.IsUninary)
                {
                    continue;
                }
                var regStr = $"^((-{schema.ShortName})|(--{schema.Name}))[:=]?";
                var regex = new Regex(regStr);
                var result = Arguments.FirstOrDefault(a => regex.IsMatch(a));
                if (!string.IsNullOrWhiteSpace(result))
                {
                    $"Found argument {{f:Green}}{schema.Name}{{f:d}}, value: {{f:Green}}{regex.Replace(result, "")}{{f:d}}"
                        .PrettyPrint(ConsoleColor.White);
                }
            
                if (!string.IsNullOrWhiteSpace(schema.Format))
                {
                    var replaceRegexStr = $"^((-{schema.ShortName})|(--{schema.Name}))[:=]?";
                    var replaceRegex = new Regex(replaceRegexStr);
                    var valueStr = replaceRegex.Replace(result ?? "", "");
                    if (schema.IsRequired)
                    {
                        if (string.IsNullOrWhiteSpace(valueStr))
                        {
                            throw new ArgumentException($"Argument {schema.Name} must not empty.");
                        }

                        if (!Regex.IsMatch(valueStr, schema.Format))
                        {
                            throw new ArgumentException(
                                $"Input value of {schema.Name} must match format of {schema.Format}");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(valueStr) && !Regex.IsMatch(valueStr, schema.Format))
                        {
                            throw new ArgumentException(
                                $"Input value of {schema.Name} must match format of {schema.Format}");
                        }
                    }
                } 

                if (!schema.IsRequired)
                {   
                    continue;
                }

                if (string.IsNullOrWhiteSpace(result))
                {
                    throw new ArgumentException($"Missing argument {{f:Yellow}}{schema.Name}{{f:d}}.");
                }
            }
        }

        protected virtual T ReadArgument<T>(string name)
        {
            var schema = Schemas.FirstOrDefault(s => s.Name.Equals(name) || s.ShortName.Equals(name));
            if (schema == null)
            {
                throw new ArgumentException($"Argument {{f:Yellow}}{name}{{f:d}} is not defined.");
            }
            
            var regex = new Regex($"^((-{schema.ShortName})|(--{schema.Name}))([:=].*)?$");

            var result = Arguments.FirstOrDefault(a => regex.IsMatch(a));

            if (!string.IsNullOrWhiteSpace(result))
            {
                result = Regex.Replace(result.Trim(), $"^((-{schema.ShortName})|(--{schema.Name}))[:=]?", "");
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return (T) Convert.ChangeType(result.Trim(), typeof(T));
                }
                if (schema.IsRequired && !schema.IsUninary)
                {
                    throw new ArgumentException($"Argument {{f:Yellow}}{schema.Name}{{f:d}} should have a value.");
                }
                if (schema.IsUninary)
                {
                    return (T)Convert.ChangeType("true", typeof(T));
                }
                return default(T);
            }

            if (schema.IsRequired)
            {
                throw new ArgumentException($"Missing argument {{f:Yellow}}{name}{{f:d}}.");
            }

            return default(T);
        }

        protected virtual bool HasArgument(string argument)
        {
            var scheme = Schemas.First(s => s.Name.Equals(argument) || s.ShortName.Equals(argument));
            if (scheme == null)
            {
                return false;
            }
            return Arguments.Any(arg =>
            {
                var regex = new Regex($"((-{scheme.ShortName})|(--{scheme.Name}))([:=].+)?");
                return !string.IsNullOrWhiteSpace(arg) && regex.IsMatch(arg);
            });
        }

        public virtual void Help()
        {
            var commands = string.Empty;
            if (Schemas != null)
            {
                foreach (var schema in Schemas)
                {
                    var name = string.IsNullOrWhiteSpace(schema.ShortName)
                        ? $"-{schema.Name}"
                        : $"-{schema.ShortName}|--{schema.Name}:<{schema.Name}>";

                    if (!schema.IsRequired)
                    {
                        name = $"[{name}]";
                    }
                    commands += $" {name}";
                }
            }
            
            $"{{f:Green}}Command:{{f:d}} {CommandName} {commands}".PrettyPrint(ConsoleColor.White);
            $"{{f:Green}}Description:{{f:d}} {Description}".PrettyPrint(ConsoleColor.White);
            if (Schemas != null)
            {
                foreach (var schema in Schemas)
                {
                    var isRequired = schema.IsRequired ? "{f:Red}Required{f:d}" : "";
                    $"\t{{f:Green}}{schema.ShortName}|{schema.Name}{{f:d}}{{t:30}}{isRequired}{{t:20}}{schema.Description}".PrettyPrint(ConsoleColor.White);
                }
            }
            
            @"
{f:Yellow}*** Notes:{f:d}
    {f:Green}[{option}]{f:d}{t:40}The option is not required.
".PrettyPrint(ConsoleColor.White);
        }
    }
}
