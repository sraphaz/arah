/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'assets-global.website-files.com',
        pathname: '/**',
      },
      {
        protocol: 'https',
        hostname: 'github.githubassets.com',
        pathname: '/**',
      },
    ],
    unoptimized: true // Necessário para static export - desabilita otimização que requer servidor
  },
  // Enable reading markdown files from docs directory
  pageExtensions: ['ts', 'tsx', 'js', 'jsx', 'md', 'mdx'],
  // Para GitHub Pages: export estático
  output: process.env.NEXT_EXPORT === 'true' ? 'export' : undefined,
  // Base path para servir em /wiki dentro do devportal.araponga.app
  basePath: '/wiki',
  trailingSlash: true,
  // Asset prefix também precisa do basePath
  assetPrefix: '/wiki',
  // Desabilita RSC (React Server Components) para evitar problemas com basePath
  // e prefetch de arquivos .txt que não existem em modo estático
  experimental: {
    optimizePackageImports: [],
  },
  // Configuração para evitar problemas com RSC e basePath
  // Em modo estático, RSC não funciona corretamente com basePath
  reactStrictMode: true,
};

export default nextConfig;
