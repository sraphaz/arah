"use client";

import { useEffect, useRef, useState } from "react";

interface MermaidDiagramProps {
  code: string;
  id?: string;
}

export function MermaidDiagram({ code, id }: MermaidDiagramProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const svgRef = useRef<SVGSVGElement | null>(null);
  const renderedRef = useRef<boolean>(false);
  const [isFullscreen, setIsFullscreen] = useState(false);
  const [zoom, setZoom] = useState(1);
  const [pan, setPan] = useState({ x: 0, y: 0 });
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState({ x: 0, y: 0 });
  const fullscreenContainerRef = useRef<HTMLDivElement>(null);
  const fullscreenSvgRef = useRef<SVGSVGElement | null>(null);

  useEffect(() => {
    if (!containerRef.current || renderedRef.current) return;

    const renderMermaid = async () => {
      try {
        // Carrega Mermaid dinamicamente
        const mermaid = (await import("mermaid")).default;
        
        // Configuracao do Mermaid no estilo do devportal
        mermaid.initialize({
          startOnLoad: false,
          theme: "dark",
          themeVariables: {
            primaryColor: "#4dd4a8", // Verde agua do devportal
            primaryTextColor: "#ffffff",
            primaryBorderColor: "#4dd4a8",
            lineColor: "#7dd3ff", // Azul claro do devportal
            secondaryColor: "#2a2a2a",
            tertiaryColor: "#3a3a3a",
            background: "#1a1a1a",
            mainBkg: "#2a2a2a",
            secondBkg: "#3a3a3a",
            textColor: "#ffffff",
            border1: "#4dd4a8",
            border2: "#7dd3ff",
            noteBkgColor: "#2a2a2a",
            noteTextColor: "#ffffff",
            noteBorderColor: "#4dd4a8",
            actorBorder: "#4dd4a8",
            actorBkg: "#2a2a2a",
            actorTextColor: "#ffffff",
            actorLineColor: "#7dd3ff",
            signalColor: "#7dd3ff",
            signalTextColor: "#ffffff",
            labelBoxBkgColor: "#2a2a2a",
            labelBoxBorderColor: "#4dd4a8",
            labelTextColor: "#ffffff",
            loopTextColor: "#ffffff",
            activationBorderColor: "#4dd4a8",
            activationBkgColor: "#2a2a2a",
            sequenceNumberColor: "#ffffff",
          },
          flowchart: {
            nodeSpacing: 50,
            rankSpacing: 80,
            curve: "basis",
            padding: 20,
          },
          fontFamily: "var(--font-inter), system-ui, sans-serif",
        });

        // Gera ID unico se nao fornecido
        const diagramId = id || `mermaid-${Math.random().toString(36).substr(2, 9)}`;
        
        // Renderiza o diagrama usando a API moderna do Mermaid
        const { svg } = await mermaid.render(diagramId, code);
        if (containerRef.current) {
          containerRef.current.innerHTML = svg;
          const svgElement = containerRef.current.querySelector('svg');
          if (svgElement) {
            svgRef.current = svgElement;
            // Estiliza o SVG
            svgElement.style.maxWidth = '100%';
            svgElement.style.height = 'auto';
          }
          renderedRef.current = true;
        }
      } catch (error) {
        console.error("Erro ao renderizar diagrama Mermaid:", error);
        if (containerRef.current) {
          containerRef.current.innerHTML = `<pre class="text-red-400 p-4 bg-red-900/20 rounded">Erro ao renderizar diagrama Mermaid: ${error}</pre>`;
        }
      }
    };

    renderMermaid();
  }, [code, id]);

  // Efeito para renderizar SVG no modal fullscreen
  useEffect(() => {
    if (isFullscreen && fullscreenContainerRef.current && svgRef.current) {
      // Limpa o container antes de adicionar o clone
      fullscreenContainerRef.current.innerHTML = '';
      
      // Clona o SVG profundamente para evitar problemas de referência
      const svgClone = svgRef.current.cloneNode(true) as SVGSVGElement;
      
      // Remove IDs duplicados que podem causar conflitos
      const allElements = svgClone.querySelectorAll('[id]');
      allElements.forEach((el) => {
        if (el.id) {
          el.id = `${el.id}-fullscreen`;
        }
      });
      
      fullscreenContainerRef.current.appendChild(svgClone);
      fullscreenSvgRef.current = svgClone;
      
      // Aplica transformacoes iniciais
      if (fullscreenSvgRef.current) {
        fullscreenSvgRef.current.style.transform = `translate(${pan.x}px, ${pan.y}px) scale(${zoom})`;
        fullscreenSvgRef.current.style.transformOrigin = 'center center';
        fullscreenSvgRef.current.style.transition = 'transform 0.1s ease-out';
      }
    }
  }, [isFullscreen]);

  // Atualiza transformacoes quando zoom ou pan mudam
  useEffect(() => {
    if (fullscreenSvgRef.current && isFullscreen) {
      fullscreenSvgRef.current.style.transform = `translate(${pan.x}px, ${pan.y}px) scale(${zoom})`;
    }
  }, [zoom, pan, isFullscreen]);

  const handleZoomIn = () => {
    setZoom(prev => Math.min(prev + 0.2, 3));
  };

  const handleZoomOut = () => {
    setZoom(prev => Math.max(prev - 0.2, 0.5));
  };

  const handleResetZoom = () => {
    setZoom(1);
    setPan({ x: 0, y: 0 });
  };

  const handleMouseDown = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.button === 0) { // Botao esquerdo
      setIsDragging(true);
      setDragStart({ x: e.clientX - pan.x, y: e.clientY - pan.y });
    }
  };

  const handleMouseMove = (e: MouseEvent) => {
    if (isDragging) {
      setPan({
        x: e.clientX - dragStart.x,
        y: e.clientY - dragStart.y,
      });
    }
  };

  const handleMouseUp = () => {
    setIsDragging(false);
  };

  const handleWheel = (e: WheelEvent) => {
    e.preventDefault();
    e.stopPropagation();
    const delta = e.deltaY > 0 ? -0.1 : 0.1;
    setZoom(prev => Math.max(0.5, Math.min(3, prev + delta)));
  };

  const handleClose = () => {
    setIsFullscreen(false);
    setZoom(1);
    setPan({ x: 0, y: 0 });
    setIsDragging(false);
  };

  // Fechar com ESC e gerenciar event listeners para drag/zoom
  useEffect(() => {
    if (isFullscreen) {
      const handleEsc = (e: KeyboardEvent) => {
        if (e.key === 'Escape') {
          handleClose();
        }
      };

      // Event listeners para mouse e wheel (fora do React para evitar passive listeners)
      const mouseMoveHandler = (e: MouseEvent) => {
        e.preventDefault();
        handleMouseMove(e);
      };
      const mouseUpHandler = () => {
        handleMouseUp();
      };
      const wheelHandler = (e: WheelEvent) => {
        handleWheel(e);
      };

      document.addEventListener('keydown', handleEsc);
      document.addEventListener('mousemove', mouseMoveHandler, { passive: false });
      document.addEventListener('mouseup', mouseUpHandler);
      document.addEventListener('wheel', wheelHandler, { passive: false });
      document.body.style.overflow = 'hidden';

      return () => {
        document.removeEventListener('keydown', handleEsc);
        document.removeEventListener('mousemove', mouseMoveHandler);
        document.removeEventListener('mouseup', mouseUpHandler);
        document.removeEventListener('wheel', wheelHandler);
        document.body.style.overflow = '';
      };
    }
  }, [isFullscreen, isDragging, dragStart]);

  return (
    <>
      <div className="relative my-8 group">
        <div 
          ref={containerRef} 
          className="mermaid-diagram-container p-4 bg-forest-900/50 dark:bg-forest-950 rounded-xl border border-forest-800 dark:border-forest-800 overflow-x-auto"
          style={{ minHeight: "300px" }}
        />
        {/* Botao de tela cheia */}
        <button
          onClick={(e) => {
            e.preventDefault();
            e.stopPropagation();
            setIsFullscreen(true);
          }}
          className="absolute top-2 right-2 bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded-lg px-3 py-2 text-sm font-semibold shadow-lg transition-all opacity-0 group-hover:opacity-100 z-10 flex items-center gap-2"
          aria-label="Abrir em tela cheia"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3" />
          </svg>
          Tela Cheia
        </button>
      </div>
      
      {isFullscreen && (
        <div 
          className="fixed inset-0 z-[9999] bg-forest-950/95 dark:bg-forest-950 backdrop-blur-sm"
          onClick={(e) => {
            if (e.target === e.currentTarget) {
              handleClose();
            }
          }}
          role="dialog"
          aria-modal="true"
          aria-label="Diagrama Mermaid em tela cheia"
        >
          {/* Botao de fechar */}
          <button
            onClick={(e) => {
              e.preventDefault();
              e.stopPropagation();
              handleClose();
            }}
            className="fixed top-4 right-4 z-[10000] bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded-full w-12 h-12 flex items-center justify-center shadow-lg transition-all hover:scale-110"
            aria-label="Fechar"
          >
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <line x1="18" y1="6" x2="6" y2="18"></line>
              <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
          </button>

          {/* Controles de zoom */}
          <div className="fixed top-4 left-4 z-[10000] flex flex-col gap-2 bg-forest-900/90 dark:bg-forest-800/90 rounded-lg p-2 shadow-lg border border-forest-800">
            <button
              onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
                handleZoomIn();
              }}
              className="px-3 py-2 bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded font-semibold transition-all"
              aria-label="Zoom in"
            >
              +
            </button>
            <button
              onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
                handleZoomOut();
              }}
              className="px-3 py-2 bg-[#4dd4a8] hover:bg-[#5ee5b9] text-forest-950 rounded font-semibold transition-all"
              aria-label="Zoom out"
            >
              −
            </button>
            <button
              onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
                handleResetZoom();
              }}
              className="px-3 py-2 bg-[#7dd3ff] hover:bg-[#8de3ff] text-forest-950 rounded text-xs font-semibold transition-all"
              aria-label="Resetar zoom"
            >
              Reset
            </button>
            <div className="text-xs text-forest-300 text-center px-2 py-1">
              {Math.round(zoom * 100)}%
            </div>
          </div>

          {/* Instrucoes */}
          <div className="fixed bottom-4 left-1/2 transform -translate-x-1/2 z-[10000] bg-forest-900/90 dark:bg-forest-800/90 rounded-lg px-4 py-2 text-sm text-forest-300 border border-forest-800">
            <span>Arraste para mover • Roda do mouse para zoom • ESC para fechar</span>
          </div>

          {/* Container do SVG com zoom e pan */}
          <div
            className="w-full h-full flex items-center justify-center overflow-hidden relative"
            onMouseDown={(e) => {
              e.preventDefault();
              e.stopPropagation();
              handleMouseDown(e);
            }}
            style={{ cursor: isDragging ? 'grabbing' : 'grab' }}
          >
            <div
              ref={fullscreenContainerRef}
              className="flex items-center justify-center"
              onClick={(e) => e.stopPropagation()}
            />
          </div>
        </div>
      )}
    </>
  );
}
