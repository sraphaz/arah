"use client";

import { ReactNode, useState, createContext, useContext } from "react";

interface TabsContextValue {
  activeTab: string;
  setActiveTab: (tab: string) => void;
}

const TabsContext = createContext<TabsContextValue | undefined>(undefined);

interface TabsProps {
  children: ReactNode;
  defaultValue: string;
}

export function Tabs({ children, defaultValue }: TabsProps) {
  const [activeTab, setActiveTab] = useState(defaultValue);

  return (
    <TabsContext.Provider value={{ activeTab, setActiveTab }}>
      <div className="tabs-container my-6">{children}</div>
    </TabsContext.Provider>
  );
}

interface TabsListProps {
  children: ReactNode;
}

export function TabsList({ children }: TabsListProps) {
  return (
    <div className="tabs-list flex gap-2 border-b-2 border-forest-200 dark:border-forest-800 mb-6 overflow-x-auto">
      {children}
    </div>
  );
}

interface TabsTriggerProps {
  value: string;
  children: ReactNode;
  icon?: string;
}

export function TabsTrigger({ value, children, icon }: TabsTriggerProps) {
  const context = useContext(TabsContext);
  if (!context) throw new Error("TabsTrigger must be used within Tabs");

  const { activeTab, setActiveTab } = context;
  const isActive = activeTab === value;

  return (
    <button
      onClick={() => setActiveTab(value)}
      className={`tabs-trigger px-6 py-3 font-medium text-sm transition-all duration-200 relative ${
        isActive
          ? "text-forest-900 dark:text-forest-50"
          : "text-forest-600 dark:text-forest-400 hover:text-forest-900 dark:hover:text-forest-200"
      }`}
    >
      <span className="flex items-center gap-2 whitespace-nowrap">
        {icon && <span>{icon}</span>}
        {children}
      </span>
      {isActive && (
        <span className="absolute bottom-0 left-0 right-0 h-0.5 bg-forest-600 dark:bg-[#4dd4a8] rounded-t-full" />
      )}
    </button>
  );
}

interface TabsContentProps {
  value: string;
  children: ReactNode;
}

export function TabsContent({ value, children }: TabsContentProps) {
  const context = useContext(TabsContext);
  if (!context) throw new Error("TabsContent must be used within Tabs");

  const { activeTab } = context;
  if (activeTab !== value) return null;

  return (
    <div className="tabs-content animation-fade-in">
      {children}
    </div>
  );
}
