import { NextRequest, NextResponse } from 'next/server';
import { readFile } from 'fs/promises';
import { join, normalize, relative } from 'path';

export async function GET(request: NextRequest) {
  try {
    const searchParams = request.nextUrl.searchParams;
    const filePath = searchParams.get('file');

    if (!filePath) {
      return NextResponse.json({ error: 'File path is required' }, { status: 400 });
    }

    // Validação de segurança: apenas arquivos .yaml e .yml
    if (!filePath.endsWith('.yaml') && !filePath.endsWith('.yml')) {
      return NextResponse.json({ error: 'Invalid file type' }, { status: 400 });
    }

    // Normaliza o caminho e previne path traversal
    const normalizedPath = normalize(filePath).replace(/\\/g, '/');
    if (normalizedPath.includes('..')) {
      return NextResponse.json({ error: 'Invalid file path' }, { status: 400 });
    }

    const docsBasePath = join(process.cwd(), "..", "..", "docs");
    const fullPath = join(docsBasePath, normalizedPath);
    
    // Verifica se o arquivo está dentro da pasta docs (segurança adicional)
    const relativePath = relative(docsBasePath, fullPath);
    if (relativePath.startsWith('..') || relativePath.includes('..')) {
      return NextResponse.json({ error: 'Invalid file path' }, { status: 400 });
    }

    const fileContents = await readFile(fullPath, "utf8");
    const fileName = normalizedPath.split('/').pop() || filePath;

    return new NextResponse(fileContents, {
      headers: {
        'Content-Type': 'application/x-yaml',
        'Content-Disposition': `attachment; filename="${fileName}"`,
      },
    });
  } catch (error) {
    console.error('Error downloading YAML file:', error);
    return NextResponse.json({ error: 'File not found' }, { status: 404 });
  }
}
