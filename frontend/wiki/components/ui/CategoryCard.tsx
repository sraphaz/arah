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
    <div className="category-card group">
      <div className="glass-card__content">
        <h2 className="text-xl md:text-2xl font-bold text-forest-900 dark:text-forest-50 mb-6 flex items-center gap-3 pb-4 border-b-2 border-forest-200/60 dark:border-forest-800/60">
          {emoji && <span className="text-2xl md:text-3xl opacity-90 group-hover:opacity-100 group-hover:scale-110 transition-all duration-300">{emoji}</span>}
          <span>{titleWithoutEmoji}</span>
        </h2>
        <ul className="space-y-1.5">
          {docs.map((doc) => {
            const docSlug = doc.path.replace(".md", "");
            return (
              <li key={doc.path}>
                <Link
                  href={`/docs/${docSlug}`}
                  className="doc-link group"
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
