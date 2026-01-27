'use client';

interface YamlDownloadButtonProps {
  fileName: string;
  content: string;
}

export function YamlDownloadButton({ fileName, content }: YamlDownloadButtonProps) {
  const handleDownload = () => {
    // Cria blob e faz download via JavaScript (compatível com static export)
    const blob = new Blob([content], { type: 'application/x-yaml' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  return (
    <div className="mb-6 flex flex-wrap gap-4 items-center">
      <button
        onClick={handleDownload}
        className="btn-secondary inline-flex items-center gap-2"
      >
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
        </svg>
        Baixar {fileName}
      </button>
      <span className="text-sm text-forest-600 dark:text-forest-400">
        Arquivo YAML - Contrato OpenAPI
      </span>
    </div>
  );
}
