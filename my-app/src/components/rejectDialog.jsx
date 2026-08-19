import  { useState } from 'react';
import { AlertCircle, X, Send } from 'lucide-react';

export default function RejectDialog({ isOpen, onClose, onSubmit, targetName }) {
  const [reason, setReason] = useState('');

  // إذا لم يكن الديالوج مفتوحاً، لا تقم برندر أي شيء
  if (!isOpen) return null;

  const handleSubmit = () => {
    if (reason.trim() === '') return; // منع الإرسال إذا كان الحقل فارغاً
    onSubmit(reason);
    setReason(''); // تصفير الحقل بعد الإرسال
  };

  const handleClose = () => {
    setReason(''); // تصفير الحقل عند الإلغاء
    onClose();
  };

  return (
    // الخلفية الضبابية (Overlay)
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-in fade-in duration-200">
      
      {/* صندوق الديالوج */}
      <div className="bg-[#1C1C1E] border border-[#333] rounded-2xl shadow-2xl w-full max-w-md overflow-hidden animate-in zoom-in-95 duration-200">
        
        {/* الترويسة */}
        <div className="flex items-center justify-between p-5 border-b border-[#333] bg-[#18181A]">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-red-500/10 rounded-lg text-red-400">
              <AlertCircle className="w-5 h-5" />
            </div>
            <h3 className="text-lg font-bold text-white">
              Rejecting {targetName || 'Item'}
            </h3>
          </div>
          <button 
            onClick={handleClose}
            className="p-1 text-gray-400 hover:text-white hover:bg-[#333] rounded-md transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* المحتوى */}
        <div className="p-5 space-y-4">
          <p className="text-sm text-gray-300">
            Please provide a reason for rejecting <span className="font-semibold text-white">{targetName}</span>. This reason will be sent to them directly.
          </p>
          
          <div className="space-y-2">
            <label htmlFor="reason" className="text-xs font-bold text-gray-400 uppercase">
              Reason for Rejection <span className="text-red-400">*</span>
            </label>
            <textarea
              id="reason"
              value={reason}
              onChange={(e) => setReason(e.target.value)}
              placeholder="e.g., The provided documentation is incomplete..."
              className="w-full h-32 p-3 bg-[#121212] border border-[#333] rounded-xl text-white text-sm placeholder-gray-600 focus:outline-none focus:border-[#EB996E] focus:ring-1 focus:ring-[#EB996E] resize-none transition-all"
            ></textarea>
          </div>
        </div>

        {/* الأزرار السفلية */}
        <div className="flex items-center justify-end gap-3 p-5 border-t border-[#333] bg-[#18181A]">
          <button
            onClick={handleClose}
            className="px-5 py-2.5 text-sm font-semibold text-gray-300 hover:text-white hover:bg-[#333] rounded-xl transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={handleSubmit}
            disabled={reason.trim() === ''}
            className="flex items-center gap-2 px-5 py-2.5 bg-[#EB996E] hover:bg-[#d8875c] text-black font-bold text-sm rounded-xl transition-colors disabled:opacity-50 disabled:cursor-not-allowed shadow-[0_0_15px_rgba(235,153,110,0.2)]"
          >
            <Send className="w-4 h-4" /> Send Rejection
          </button>
        </div>
        
      </div>
    </div>
  );
}