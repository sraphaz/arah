import Hero from "@/components/sections/Hero";
import Problem from "@/components/sections/Problem";
import Proposal from "@/components/sections/Proposal";
import HowItWorks from "@/components/sections/HowItWorks";
import Domains from "@/components/sections/Domains";
import Visibility from "@/components/sections/Visibility";
import Value from "@/components/sections/Value";
import Architecture from "@/components/sections/Architecture";
import Roadmap from "@/components/sections/Roadmap";
import CTA from "@/components/sections/CTA";
import Footer from "@/components/sections/Footer";
import Divider from "@/components/ui/Divider";

export default function Home() {
  return (
    <main>
      <Hero />
      <Divider />
      <Problem />
      <Divider />
      <Proposal />
      <Divider />
      <HowItWorks />
      <Divider />
      <Domains />
      <Divider />
      <Visibility />
      <Divider />
      <Value />
      <Divider />
      <Architecture />
      <Divider />
      <Roadmap />
      <Divider />
      <CTA />
      <Divider />
      <Footer />
    </main>
  );
}
