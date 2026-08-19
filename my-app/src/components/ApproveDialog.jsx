import { AlertTriangle, X, CheckCircle2 } from 'lucide-react';

export default function ApproveDialog({ isOpen, onClose, onConfirm, targetName }) {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
      <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl shadow-2xl w-full max-w-md overflow-hidden">
        <div className="flex items-center justify-between p-5 border-b border-[#333] bg-[#18181A]">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-amber-500/10 rounded-lg text-amber-400">
              <AlertTriangle className="w-5 h-5" />
            </div>
            <h3 className="text-lg font-bold text-white">
              Approve {targetName || 'Item'}?
            </h3>
          </div>
          <button
            onClick={onClose}
            className="p-1 text-gray-400 hover:text-white hover:bg-[#333] rounded-md transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="p-5 space-y-4">
          <p className="text-sm text-gray-300">
            Are you sure you want to approve <span className="font-semibold text-white">{targetName}</span>?
            This action will remove it from the pending list.
          </p>
        </div>

        <div className="flex items-center justify-end gap-3 p-5 border-t border-[#333] bg-[#18181A]">
          <button
            onClick={onClose}
            className="px-5 py-2.5 text-sm font-semibold text-gray-300 hover:text-white hover:bg-[#333] rounded-xl transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            className="flex items-center gap-2 px-5 py-2.5 bg-[#91B3FA] hover:bg-[#7fa1e8] text-black font-bold text-sm rounded-xl transition-colors"
          >
            <CheckCircle2 className="w-4 h-4" /> Confirm
          </button>
        </div>
      </div>
    </div>
  );
}
