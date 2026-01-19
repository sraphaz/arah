import Image from "next/image";

type SocialIconProps = {
  platform: "github" | "discord";
  size?: number;
  className?: string;
};

/**
 * Componente reutilizável para ícones de redes sociais
 * Suporta GitHub (preto/branco baseado no tema) e Discord (azul)
 */
export function SocialIcon({ platform, size = 20, className = "" }: SocialIconProps) {
  const iconPaths = {
    github: {
      light: "/wiki/GitHub_Invertocat_Black.svg",
      dark: "/wiki/GitHub_Invertocat_White.svg",
    },
    discord: {
      light: "/wiki/discord_icon.svg",
      dark: "/wiki/discord_icon.svg", // Discord sempre azul
    },
  };

  const icons = iconPaths[platform];

  return (
    <>
      {/* Light mode icon */}
      <Image
        src={icons.light}
        alt={`${platform} icon`}
        width={size}
        height={size}
        className={`dark:hidden ${className}`}
        style={{ width: size, height: size }}
      />
      {/* Dark mode icon */}
      <Image
        src={icons.dark}
        alt={`${platform} icon`}
        width={size}
        height={size}
        className={`hidden dark:block ${className}`}
        style={{ width: size, height: size }}
      />
    </>
  );
}
