"use client";

import { ReactNode, useState, useRef, useEffect } from "react";

interface TooltipProps {
  content: string | ReactNode;
  children: ReactNode;
  position?: "top" | "bottom" | "left" | "right";
}

export function Tooltip({ content, children, position = "top" }: TooltipProps) {
  const [isVisible, setIsVisible] = useState(false);
  const [tooltipPosition, setTooltipPosition] = useState({ top: 0, left: 0 });
  const triggerRef = useRef<HTMLDivElement>(null);
  const tooltipRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (isVisible && triggerRef.current && tooltipRef.current) {
      const triggerRect = triggerRef.current.getBoundingClientRect();
      const tooltipRect = tooltipRef.current.getBoundingClientRect();
      const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
      const scrollLeft = window.pageXOffset || document.documentElement.scrollLeft;

      let top = 0;
      let left = 0;

      switch (position) {
        case "top":
          top = triggerRect.top + scrollTop - tooltipRect.height - 8;
          left = triggerRect.left + scrollLeft + triggerRect.width / 2 - tooltipRect.width / 2;
          break;
        case "bottom":
          top = triggerRect.bottom + scrollTop + 8;
          left = triggerRect.left + scrollLeft + triggerRect.width / 2 - tooltipRect.width / 2;
          break;
        case "left":
          top = triggerRect.top + scrollTop + triggerRect.height / 2 - tooltipRect.height / 2;
          left = triggerRect.left + scrollLeft - tooltipRect.width - 8;
          break;
        case "right":
          top = triggerRect.top + scrollTop + triggerRect.height / 2 - tooltipRect.height / 2;
          left = triggerRect.right + scrollLeft + 8;
          break;
      }

      setTooltipPosition({ top, left });
    }
  }, [isVisible, position]);

  return (
    <>
      <div
        ref={triggerRef}
        onMouseEnter={() => setIsVisible(true)}
        onMouseLeave={() => setIsVisible(false)}
        onFocus={() => setIsVisible(true)}
        onBlur={() => setIsVisible(false)}
        className="inline-block"
      >
        {children}
      </div>
      {isVisible && (
        <div
          ref={tooltipRef}
          className="tooltip-content fixed z-50 px-3 py-2 text-sm rounded-lg shadow-lg pointer-events-none"
          style={{
            top: `${tooltipPosition.top}px`,
            left: `${tooltipPosition.left}px`,
          }}
        >
          <div className="bg-forest-900 dark:bg-forest-50 text-forest-50 dark:text-forest-900 px-3 py-2 rounded-lg shadow-xl">
            {content}
          </div>
        </div>
      )}
    </>
  );
}
