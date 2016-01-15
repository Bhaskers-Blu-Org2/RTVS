using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.R.Package.History {
    internal sealed class SinglelineRHistoryEntries : RHistoryEntries {
        public override bool IsMultiline => false;

        public SinglelineRHistoryEntries() {
        }

        public SinglelineRHistoryEntries(IRHistoryEntries entries) {
            if (entries.IsMultiline) {
                CloneFromMultiline(entries);
            } else { 
                CloneEntries(entries);
            }
        }

        public override void Add(ITrackingSpan entrySpan) {
            AddSinglelineEntries(entrySpan);
        }

        private void CloneFromMultiline(IRHistoryEntries existingEntries) {
            foreach (var existingEntry in existingEntries.GetEntries()) {
                var entriesGroup = AddSinglelineEntries(existingEntry.EntrySpan);
                foreach (var newEntry in entriesGroup) {
                    newEntry.IsSelected = existingEntry.IsSelected;
                }
            }
        }

        private IList<IRHistoryEntry> AddSinglelineEntries(ITrackingSpan entrySpan) {
            var snapshot = entrySpan.TextBuffer.CurrentSnapshot;
            var snapshotEntrySpan = entrySpan.GetSpan(snapshot);
            var spans = snapshot.Lines
                .Where(l => snapshotEntrySpan.Contains(l.Extent))
                .Select(l => snapshot.CreateTrackingSpan(l.Start, l.Length, SpanTrackingMode.EdgeExclusive));

            var entries = new List<IRHistoryEntry>();
            foreach (var span in spans) {
                entries.Add(AddEntry(entrySpan, span));
            }
            return entries;
        }
    }
}