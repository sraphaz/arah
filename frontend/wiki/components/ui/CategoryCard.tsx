import Link from "next/link";

interface Doc {
  name: string;
  path: string;
}

interface CategoryCardProps {
  category: string;
  docs: Doc[];
}

export function CategoryCard({ category, docs }: CategoryCardProps) {
  // Remove apenas emojis do início do título (mantém texto completo)
  // Remove padrão: emoji + espaço no início
  const titleWithoutEmoji = category.replace(/^[\p{Emoji_Presentation}\p{Emoji}\u{1F300}-\u{1F9FF}]\s*/u, "").trim();

  return (
    <div className="category-card">
      <div className="category-card__content">
        {/* Título da categoria - Tipografia enterprise-level */}
        <h3 className="text-xl md:text-2xl font-bold text-forest-900 dark:text-forest-50 mb-6 pb-3 border-b border-forest-200/40 dark:border-forest-800/40">
          {titleWithoutEmoji || category}
        </h3>
        {/* Grid horizontal para itens - aproveita espaço horizontal */}
        {/* Mobile: 1 col, Tablet: 2 cols, Desktop: 2 cols (itens alinhados horizontalmente) */}
        <ul className="grid grid-cols-1 sm:grid-cols-2 gap-3 flex-1">
          {docs.map((doc) => {
            const docSlug = doc.path.replace(".md", "");
            return (
              <li key={doc.path}>
                <Link
                  href={`/docs/${docSlug}`}
                  className="doc-link"
                >
                  <span className="doc-link-arrow">→</span>
                  <span className="flex-1">{doc.name}</span>
                </Link>
              </li>
            );
          })}
        </ul>
      </div>
    </div>
  );
}
