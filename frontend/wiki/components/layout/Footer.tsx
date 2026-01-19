import Image from "next/image";
import { SocialIcon } from "../ui/SocialIcon";

export function Footer() {
  return (
    <footer className="border-t border-forest-200/80 dark:border-forest-900/80 bg-white/80 dark:bg-forest-950/80 backdrop-blur-xl mt-32">
      <div className="container-max py-16">
        <div className="text-center space-y-6">
          <p className="text-forest-800 dark:text-forest-200 text-lg md:text-xl font-medium leading-relaxed max-w-3xl mx-auto">
            Wiki Araponga — Documentação completa da plataforma digital comunitária
            orientada ao território
          </p>
          <div className="flex flex-wrap items-center justify-center gap-6 text-sm">
            <a
              href="https://araponga.app"
              target="_blank"
              rel="noopener noreferrer"
              className="footer-link flex items-center gap-2 transition-transform hover:scale-105"
            >
              <Image
                src="/wiki/icon.png"
                alt="Araponga"
                width={20}
                height={20}
                className="opacity-90 hover:opacity-100 transition-opacity"
              />
              <span>Site Oficial</span>
            </a>
            <span className="text-forest-400 dark:text-forest-600">•</span>
            <a
              href="https://github.com/sraphaz/araponga"
              target="_blank"
              rel="noopener noreferrer"
              className="footer-link flex items-center gap-2 transition-transform hover:scale-105"
            >
              <SocialIcon platform="github" size={20} />
              <span>Contribuir no GitHub</span>
            </a>
            <span className="text-forest-400 dark:text-forest-600">•</span>
            <a
              href="https://discord.gg/auwqN8Yjgw"
              target="_blank"
              rel="noopener noreferrer"
              className="footer-link flex items-center gap-2 transition-transform hover:scale-105"
            >
              <SocialIcon platform="discord" size={20} />
              <span>Discord da Comunidade</span>
            </a>
          </div>
          <p className="text-forest-500 dark:text-forest-400 text-xs mt-8 tracking-wide font-medium">
            Território como referência • Comunidade como prioridade • Tecnologia como ferramenta
          </p>
        </div>
      </div>
    </footer>
  );
}
