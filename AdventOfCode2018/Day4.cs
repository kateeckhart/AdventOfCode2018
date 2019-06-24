using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day4 : ISolution
    {
        private static Regex ParsingRegex { get; } =
            new Regex(
                @"\[(?<Year>\d{4})-(?<Month>\d{2})-(?<Day>\d{2}) (?<Hour>\d{2}):(?<Minute>\d{2})] (?<Command>.+)");

        private static Regex GuardShiftParse { get; } = new Regex(@"Guard #(?<Id>\d+) begins shift");
        public int DayN => 4;

        public (string, string) GetAns(string[] rawInput)
        {
            var (guards, events) = ParseInput(rawInput);
            var state = new State();
            foreach (var event0 in events) event0.Run(ref state);

            return (AnalyzePart1(guards).ToString(), AnalyzePart2(guards).ToString());
        }

        private static int AnalyzePart1(IDictionary<int, Guard> guards)
        {
            Guard sleepiestGuard = null;
            var sleepiestGuardId = 0;
            var minutesSlept = 0;
            foreach (var (id, guard) in guards)
            {
                if (guard.MinutesAsleep <= minutesSlept) continue;
                minutesSlept = guard.MinutesAsleep;
                sleepiestGuard = guard;
                sleepiestGuardId = id;
            }

            var sleepByMinute = sleepiestGuard.SleepByMinute;
            var sleepiestMinute = -1;
            minutesSlept = 0;
            for (var i = 0; i < 60; i++)
            {
                if (sleepByMinute[i] <= minutesSlept) continue;
                sleepiestMinute = i;
                minutesSlept = sleepByMinute[i];
            }

            return sleepiestGuardId * sleepiestMinute;
        }

        private static int AnalyzePart2(IDictionary<int, Guard> guards)
        {
            var mostConsistentGuardId = 0;
            var minuteSlept = -1;
            var timesSlept = -1;
            foreach (var (id, guard) in guards)
            {
                var sleepByMinute = guard.SleepByMinute;
                for (var i = 0; i < 60; i++)
                {
                    if (sleepByMinute[i] <= timesSlept) continue;
                    timesSlept = sleepByMinute[i];
                    minuteSlept = i;
                    mostConsistentGuardId = id;
                }
            }

            return mostConsistentGuardId * minuteSlept;
        }

        private static (IDictionary<int, Guard>, Event[]) ParseInput(string[] input)
        {
            var events = new Event[input.Length];
            var guards = new Dictionary<int, Guard>();
            for (var index = 0; index < input.Length; index++)
            {
                var match = ParsingRegex.Match(input[index]);

                var year = int.Parse(match.Groups["Year"].Value);
                var month = int.Parse(match.Groups["Month"].Value);
                var day = int.Parse(match.Groups["Day"].Value);
                var hour = int.Parse(match.Groups["Hour"].Value);
                var minute = int.Parse(match.Groups["Minute"].Value);

                var time = new Time(year, month, day, hour, minute);

                var rawCommand = match.Groups["Command"].Value;

                ICommand command;

                switch (rawCommand)
                {
                    case "falls asleep":
                        command = Sleep.Instance;
                        break;
                    case "wakes up":
                        command = Wake.Instance;
                        break;
                    default:
                        var rawGuardShift = GuardShiftParse.Match(rawCommand);
                        var id = int.Parse(rawGuardShift.Groups["Id"].Value);
                        if (!guards.TryGetValue(id, out var guard))
                        {
                            guard = new Guard();
                            guards[id] = guard;
                        }

                        command = new GuardShift(guard);
                        break;
                }

                events[index] = new Event(command, time);
            }

            Array.Sort(events);
            return (guards, events);
        }

        private class Event : IComparable<Event>
        {
            public Event(ICommand command, Time time)
            {
                Command = command;
                When = time;
            }

            private Time When { get; }
            private ICommand Command { get; }

            public int CompareTo(Event other)
            {
                if (ReferenceEquals(this, other)) return 0;
                return ReferenceEquals(null, other) ? 1 : When.CompareTo(other.When);
            }

            public void Run(ref State state)
            {
                Command.Run(ref state, When);
            }
        }

        private interface ICommand
        {
            void Run(ref State state, Time time);
        }

        private class Wake : ICommand
        {
            private Wake()
            {
            }

            public static Wake Instance { get; } = new Wake();

            public void Run(ref State state, Time time)
            {
                if (!state.CurrentGuard.AsleepByDay.TryGetValue(time.Day, out var asleepByDay))
                {
                    asleepByDay = new bool[60];
                    state.CurrentGuard.AsleepByDay[time.Day] = asleepByDay;
                }

                for (var i = state.SleptAt.Minute; i < time.Minute; i++)
                {
                    state.CurrentGuard.MinutesAsleep++;
                    asleepByDay[i] = true;
                }
            }
        }

        private class Sleep : ICommand
        {
            private Sleep()
            {
            }

            public static Sleep Instance { get; } = new Sleep();

            public void Run(ref State state, Time time)
            {
                state.SleptAt = time;
            }
        }

        private class GuardShift : ICommand
        {
            public GuardShift(Guard newGuard)
            {
                NewGuard = newGuard;
            }

            private Guard NewGuard { get; }

            public void Run(ref State state, Time time)
            {
                state.CurrentGuard = NewGuard;
            }
        }

        private struct Time : IComparable<Time>, IEquatable<Time>
        {
            public Time(int year, int month, int day, int hour, int minute)
            {
                Day = new Date(year, month, day);
                Hour = hour;
                Minute = minute;
            }

            public bool Equals(Time other)
            {
                return Day == other.Day && Hour == other.Hour && Minute == other.Minute;
            }

            public override bool Equals(object obj)
            {
                return obj is Time other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (Day, Hour, Minute).GetHashCode();
            }

            public int CompareTo(Time other)
            {
                var dateComparison = Day.CompareTo(other.Day);
                if (dateComparison != 0) return dateComparison;
                var hourComparison = Hour.CompareTo(other.Hour);
                return hourComparison != 0 ? hourComparison : Minute.CompareTo(other.Minute);
            }

            public Date Day { get; }
            private int Hour { get; }
            public int Minute { get; }
        }

        private struct Date : IComparable<Date>
        {
            public int CompareTo(Date other)
            {
                var yearComparison = Year.CompareTo(other.Year);
                if (yearComparison != 0) return yearComparison;
                var monthComparison = Month.CompareTo(other.Month);
                return monthComparison != 0 ? monthComparison : Day.CompareTo(other.Day);
            }

            private bool Equals(Date other)
            {
                return Year == other.Year && Month == other.Month && Day == other.Day;
            }

            public override bool Equals(object obj)
            {
                return obj is Date other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (Year, Month, Day).GetHashCode();
            }

            public static bool operator ==(Date left, Date right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Date left, Date right)
            {
                return !left.Equals(right);
            }

            public Date(int year, int month, int day)
            {
                Year = year;
                Month = month;
                Day = day;
            }

            private int Year { get; }
            private int Month { get; }
            private int Day { get; }
        }

        private struct State
        {
            public Guard CurrentGuard { get; set; }
            public Time SleptAt { get; set; }
        }

        private class Guard
        {
            public int MinutesAsleep { get; set; }
            public Dictionary<Date, bool[]> AsleepByDay { get; } = new Dictionary<Date, bool[]>();

            public int[] SleepByMinute
            {
                get
                {
                    var sleepByMinute = new int[60];
                    foreach (var (_, day) in AsleepByDay)
                        for (var i = 0; i < 60; i++)
                            if (day[i])
                                sleepByMinute[i]++;

                    return sleepByMinute;
                }
            }
        }
    }
}