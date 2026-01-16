import Hero from "@/components/sections/Hero";
import Inspiration from "@/components/sections/Inspiration";
import Problem from "@/components/sections/Problem";
import Proposal from "@/components/sections/Proposal";
import HowItWorks from "@/components/sections/HowItWorks";
import Architecture from "@/components/sections/Architecture";
import Visibility from "@/components/sections/Visibility";
import Value from "@/components/sections/Value";
import Technology from "@/components/sections/Technology";
import Features from "@/components/sections/Features";
import Improvements from "@/components/sections/Improvements";
import Roadmap from "@/components/sections/Roadmap";
import Future from "@/components/sections/Future";
import Join from "@/components/sections/Join";
import Support from "@/components/sections/Support";

export default function Home() {
  return (
    <main>
      <Hero />
      <Inspiration />
      <Problem />
      <Proposal />
      <HowItWorks />
      <Architecture />
      <Visibility />
      <Value />
      <Technology />
      <Features />
      <Improvements />
      <Roadmap />
      <Future />
      <Join />
      <Support />
    </main>
  );
}
