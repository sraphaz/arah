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
  const emojiMatch = category.match(/^(\S+)\s/);
  const emoji = emojiMatch ? emojiMatch[1] : "";
  const titleWithoutEmoji = emoji ? category.replace(/^\S+\s/, "") : category;

  return (
    <div className="category-card">
      <div className="category-card__content">
        <h2 className="text-lg font-semibold text-forest-900 dark:text-forest-50 mb-4 flex items-center gap-2">
          {emoji && <span className="text-xl opacity-80">{emoji}</span>}
          <span>{titleWithoutEmoji}</span>
        </h2>
        <ul className="space-y-1.5 flex-1 overflow-y-auto">
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
