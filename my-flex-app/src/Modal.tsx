import React from "react";

interface ModalProps {
  open: boolean;
  onClose: () => void;
  children: React.ReactNode;
  className?: string;
}

export default function Modal({ open, onClose, children,className }: ModalProps) {
  if (!open) return null;

  return (
    
    <div className={`${className??''} fixed inset-0 bg-neutral-300/75 dark:bg-gray-950/75 flex items-center justify-center z-50" role="dialog" aria-modal="true`}>
      <div className="bg-white dark:bg-gray-800 rounded-sm shadow-lg w-96 p-6 relative">
        <button onClick={onClose} className="absolute top-2 right-2 text-gray-500 hover:text-gray-700 dark:text-gray-300">
          ✕
        </button>
        {children}
      </div>
    </div>
  );
}