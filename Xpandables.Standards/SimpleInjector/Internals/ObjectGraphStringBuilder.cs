// Copyright (c) Simple Injector Contributors. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

namespace SimpleInjector.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal sealed class ObjectGraphStringBuilder
    {
        private const int IndentSize = 4;

        private readonly StringBuilder builder = new StringBuilder();
        private readonly Stack<ProducerEntry> producers = new Stack<ProducerEntry>();
        private readonly VisualizationOptions visualizationOptions;

        private ProducerEntry? stillToWriteLifestyleEntry;
        private int indentingDepth;

        public ObjectGraphStringBuilder(VisualizationOptions visualizationOptions)
        {
            this.visualizationOptions = visualizationOptions;
        }

        public override string ToString() => builder.ToString();

        internal void BeginInstanceProducer(InstanceProducer producer)
        {
            if (producers.Count > 0)
            {
                AppendLifestyle(producers.Peek());
                AppendNewLine();
            }

            producers.Push(new ProducerEntry(producer));

            Append(producer.ImplementationType.ToFriendlyName(visualizationOptions.UseFullyQualifiedTypeNames));
            Append("(");

            indentingDepth++;
        }

        internal void AppendCyclicInstanceProducer(InstanceProducer producer, bool last)
        {
            BeginInstanceProducer(producer);
            Append("/* cyclic dependency graph detected */");
            EndInstanceProducer(last);
        }

        internal void EndInstanceProducer(bool last)
        {
            var entry = producers.Pop();

            indentingDepth--;

            Append(")");

            if (!last)
            {
                Append(",");
                if (stillToWriteLifestyleEntry != null)
                {
                    AppendLifestyle(stillToWriteLifestyleEntry);
                    stillToWriteLifestyleEntry = null;
                }

                AppendLifestyle(entry);
            }

            if (!entry.LifestyleWritten)
            {
                stillToWriteLifestyleEntry = entry;
            }

            if (!producers.Any())
            {
                AppendLifestyle(stillToWriteLifestyleEntry!);
            }
        }

        private void AppendNewLine()
        {
            Append(Environment.NewLine);
            AppendIndent();
        }

        private void AppendLifestyle(ProducerEntry entry)
        {
            if (visualizationOptions.IncludeLifestyleInformation && !entry.LifestyleWritten)
            {
                Append(" // ");
                Append(entry.Producer.Lifestyle.Name);
                entry.LifestyleWritten = true;
            }
        }

        private void AppendIndent()
        {
            for (int i = 0; i < indentingDepth * IndentSize; i++)
            {
                builder.Append(' ');
            }
        }

        private void Append(string value) => builder.Append(value);

        private sealed class ProducerEntry
        {
            public ProducerEntry(InstanceProducer producer)
            {
                Producer = producer;
            }

            public InstanceProducer Producer { get; }
            public bool LifestyleWritten { get; set; }
        }
    }
}