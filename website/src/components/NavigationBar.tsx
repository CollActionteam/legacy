import React from "react";

export default function NavigationBar() {
  return (
    <header className="w-full py-4">
      <div className="flex items-center justify-between px-8">
        <img className="block h-8 w-auto" src="/logo.svg" alt="CollAction" width={131} height={32} />
        <div className="">
          <div className="flex items-center">
            <span className="px-4 py-1 text-sm text-collaction cursor-default font-semibold rounded-full text-right">
              App launch: October 31
            </span>
          </div>
        </div>
      </div>
    </header>
  );
}
