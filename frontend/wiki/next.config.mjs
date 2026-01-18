/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [],
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
};

export default nextConfig;
