import Link from "next/link";

interface DocCardProps {
  name: string;
  slug: string;
}

export function DocCard({ name, slug }: DocCardProps) {
  return (
    <Link href={`/docs/${slug}`} className="doc-link group">
      <span>→</span>
      <span className="flex-1">{name}</span>
    </Link>
  );
}
