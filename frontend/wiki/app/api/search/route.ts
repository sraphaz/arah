/**
 * API Route para busca da Wiki
 * Retorna índice de documentos para busca client-side
 */

import { NextResponse } from 'next/server';
import { generateWikiIndex } from '../../../lib/search-index';

export async function GET() {
  try {
    const index = await generateWikiIndex();
    return NextResponse.json({ index });
  } catch (error) {
    console.error('Error generating search index:', error);
    return NextResponse.json(
      { error: 'Failed to generate search index' },
      { status: 500 }
    );
  }
}
