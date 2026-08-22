import { X, Mail, CheckCircle2 } from 'lucide-react';

export default function SendConfirmDialog({ isOpen, onClose, onConfirm, targetName }) {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
      <div className="bg-[var(--color-surface)] border border-[var(--color-border)] rounded-2xl shadow-2xl w-full max-w-md overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-[var(--color-border)] bg-[var(--color-surface-alt)]">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-[var(--color-accent-soft)] rounded-lg text-[var(--color-accent)]">
              <Mail className="w-5 h-5" />
            </div>
            <h3 className="text-lg font-bold text-[var(--color-text)]">Send reply to {targetName}?</h3>
          </div>
          <button onClick={onClose} className="p-1 text-[var(--color-text-muted)] hover:text-[var(--color-text)] hover:bg-[var(--color-surface-alt)] rounded-md transition-colors">
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="p-5 space-y-4">
          <p className="text-sm text-[var(--color-text-muted)]">
            This reply will close the ticket and no further replies can be sent for it.
          </p>
        </div>

        <div className="flex items-center justify-end gap-3 p-5 border-t border-[var(--color-border)] bg-[var(--color-surface-alt)]">
          <button onClick={onClose} className="px-5 py-2.5 text-sm font-semibold text-[var(--color-text-muted)] hover:text-[var(--color-text)] hover:bg-[var(--color-surface-alt)] rounded-xl transition-colors">Cancel</button>
          <button onClick={onConfirm} className="flex items-center gap-2 px-5 py-2.5 bg-[var(--color-accent)] hover:opacity-90 text-white font-bold text-sm rounded-xl transition-colors">
            <CheckCircle2 className="w-4 h-4" /> Confirm
          </button>
        </div>
      </div>
    </div>
  );
}
