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
  // NÃO usar assetPrefix junto com basePath em static export.
  // Duplicar /wiki causa 404 em RSC payloads e quebra hidratação ("$Sreact.fragment").
  // Bug conhecido: static export + basePath → cliente pede /wiki.txt em vez de /wiki/index.txt.
  // Ver: https://github.com/vercel/next.js/issues/73427 (PR #73912).
  experimental: {
    optimizePackageImports: [],
  },
  reactStrictMode: true,
};

export default nextConfig;
