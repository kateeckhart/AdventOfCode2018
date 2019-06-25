using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2018
{
    public class Day7 : ISolution
    {
        private static Regex ParsingRegex { get; } =
            new Regex(@"Step (?<Dependency>\w) must be finished before step (?<Dependent>\w) can begin\.");

        public int DayN => 7;

        public (string, string) GetAns(string[] input)
        {
            return (RunSteps(input, 1).Item1, RunSteps(input, 5).Item2.ToString());
        }

        private static IList<Step> ParseInput(IEnumerable<string> input)
        {
            var stepMap = new Dictionary<char, Step>();
            foreach (var line in input)
            {
                var match = ParsingRegex.Match(line);
                var dependencyId = match.Groups["Dependency"].Value[0];
                var dependentId = match.Groups["Dependent"].Value[0];

                if (!stepMap.TryGetValue(dependentId, out var dependent))
                {
                    dependent = new Step(dependentId);
                    stepMap.Add(dependentId, dependent);
                }

                if (!stepMap.TryGetValue(dependencyId, out var dependency))
                {
                    dependency = new Step(dependencyId);
                    stepMap.Add(dependencyId, dependency);
                }

                dependent.Dependencies.Add(dependency);
            }

            var steps = new List<Step>(stepMap.Select(step => step.Value));
            steps.Sort();
            return steps;
        }

        private static (string, int) RunSteps(IEnumerable<string> inSteps, int workerCount)
        {
            var doneSteps = new StringBuilder();
            var steps = ParseInput(inSteps);
            var workers = new Task?[workerCount];
            var time = 0;
            while (steps.Count > 0)
            {
                var workerN = -1;
                for (var i = 0; i < workers.Length; i++)
                {
                    if (workers[i] != null) continue;
                    workerN = i;
                    break;
                }

                for (var x = 0; x < steps.Count; x++)
                {
                    if (steps[x].Dependencies.Count != 0) continue;

                    foreach (var worker in workers)
                    {
                        if (worker == null) continue;
                        if (worker.Value.CurrentStep == x) goto stepLoop;
                    }

                    workers[workerN] = new Task(x, steps[x].Id - 'A' + 61 + time);
                    var workersFull = true;
                    for (var i = workerN + 1; i < workers.Length; i++)
                    {
                        if (workers[i] != null) continue;
                        workerN = i;
                        workersFull = false;
                        break;
                    }

                    if (workersFull) break;
                    stepLoop: ;
                }

                var workerClosestToDone = -1;
                var timeToDone = int.MaxValue;

                for (var i = 0; i < workers.Length; i++)
                {
                    if (workers[i] == null) continue;
                    if (workers[i].Value.Time >= timeToDone) continue;
                    workerClosestToDone = i;
                    timeToDone = workers[i].Value.Time;
                }

                time = timeToDone;

                var doneStepIndex = workers[workerClosestToDone].Value.CurrentStep;
                workers[workerClosestToDone] = null;

                var doneStep = steps[doneStepIndex];
                doneSteps.Append(doneStep.Id);
                steps.RemoveAt(doneStepIndex);
                for (var i = 0; i < workers.Length; i++)
                {
                    if (workers[i] == null) continue;
                    var worker = workers[i].Value;
                    if (worker.CurrentStep <= doneStepIndex) continue;
                    worker.CurrentStep--;
                    workers[i] = worker;
                }

                foreach (var dependentStep in steps)
                    for (var h = 0; h < dependentStep.Dependencies.Count; h++)
                        if (dependentStep.Dependencies[h] == doneStep)
                        {
                            dependentStep.Dependencies.RemoveAt(h);
                            break;
                        }
            }

            return (doneSteps.ToString(), time);
        }

        private class Step : IComparable<Step>
        {
            public Step(char id)
            {
                Id = id;
                Dependencies = new List<Step>();
            }

            public Step(Step other)
            {
                Id = other.Id;
                Dependencies = new List<Step>(other.Dependencies);
            }

            public char Id { get; }
            public List<Step> Dependencies { get; }

            public int CompareTo(Step other)
            {
                return Id.CompareTo(other.Id);
            }
        }

        private struct Task
        {
            public Task(int currentStep, int time)
            {
                CurrentStep = currentStep;
                Time = time;
            }

            public int CurrentStep { get; set; }
            public int Time { get; }
        }
    }
}