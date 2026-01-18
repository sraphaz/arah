import Image from "next/image";

interface OfficialIconProps {
  src: string;
  alt: string;
  width?: number;
  height?: number;
  className?: string;
}

export function OfficialIcon({
  src,
  alt,
  width = 24,
  height = 24,
  className = ""
}: OfficialIconProps) {
  // Para SVGs externos, usar img diretamente (não usa Image do Next.js para evitar problemas com CORS/otimização)
  // O Next.js Image requer domínios configurados em remotePatterns e pode ter problemas com SVGs externos
  return (
    <img
      src={src}
      alt={alt}
      width={width}
      height={height}
      className={className}
      style={{ width: `${width}px`, height: `${height}px`, objectFit: "contain" }}
      loading="lazy"
    />
  );
}
