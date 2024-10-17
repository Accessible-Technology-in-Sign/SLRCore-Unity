using System;
using System.Collections.Generic;
using System.Linq;

namespace Common {

    public class FilterUnit<T> {
        public List<T> mapping;
        public float[] probabilities;

        public FilterUnit(List<T> mapping, float[] probabilities) {
            this.mapping = mapping;
            this.probabilities = probabilities;
        }
    }

    public static class ArgMaxExtension {
        public static int ArgMax<T>(this IEnumerable<T> source) where T : IComparable<T> {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }

                int maxIndex = 0;
                int currentIndex = 0;
                T maxValue = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (enumerator.Current.CompareTo(maxValue) > 0)
                    {
                        maxValue = enumerator.Current;
                        maxIndex = currentIndex;
                    }
                }

                return maxIndex;
            }
        }
    }

    public interface PredictionFilter<T> {
        public FilterUnit<T> Filter(FilterUnit<T> input);
    }

    public class PassThroughFilter<T> : PredictionFilter<T> {
        public FilterUnit<T> Filter(FilterUnit<T> input) {
            if (input.mapping.Count != input.probabilities.Length)
                throw new ArgumentException("Prediction filter received mapping that was not the same size as the input");
            if (input.mapping.Count == 0) return input;
            return input;
        }
    }
    
    // TODO: rename to 
    public class PassThroughFilterSingle<T> : PredictionFilter<T> {
        public FilterUnit<T> Filter(FilterUnit<T> input) {
            if (input.mapping.Count != input.probabilities.Length)
                throw new ArgumentException("Prediction filter received mapping that was not the same size as the input");
            if (input.mapping.Count == 0) return input;
            return new FilterUnit<T>(
                new List<T>(new []{input.mapping[input.probabilities.ArgMax()]}),
                new float[] {input.probabilities.Max()}
            );
        }
    }

    public class Thresholder<T> : PredictionFilter<T> {
        private int threshold;
        
        public Thresholder(int threshold) {this.threshold = threshold;}
        
        public FilterUnit<T> Filter(FilterUnit<T> input) {
            if (input.mapping.Count != input.probabilities.Length)
                throw new ArgumentException("Prediction filter received mapping that was not the same size as the input");
            if (input.mapping.Count == 0) return input;
            var filteredMapping = input.mapping
                .Where((value, idx) => input.probabilities[idx] > threshold)
                .ToList();

            var filteredProbabilities = input.probabilities
                .Where(value => value > threshold)
                .ToArray();

            return new FilterUnit<T>(filteredMapping, filteredProbabilities);
        }
    }

    public class FocusSublistFilter<T> : PredictionFilter<T> {
        private List<T> focusSublist;
        
        public FilterUnit<T> Filter(FilterUnit<T> input) {
            if (input.mapping.Count != input.probabilities.Length)
                throw new ArgumentException("Prediction filter received mapping that was not the same size as the input");
            if (input.mapping.Count == 0) return input;
            
            var indices = input.mapping
                .Select((value, idx) => focusSublist.Contains(value) ? idx : -1)
                .Where(idx => idx != -1)
                .ToList();

            var filteredMapping = indices.Select(idx => input.mapping[idx]).ToList();
            var filteredProbabilities = indices.Select(idx => input.probabilities[idx]).ToArray();

            return new FilterUnit<T>(filteredMapping, filteredProbabilities);
        }
    }
}