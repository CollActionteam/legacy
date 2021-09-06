import type { NextPage } from "next";

import Footer from "../components/Footer";

const Home: NextPage = () => {
  return (
    <div className="bg-white text-black-400">
      <header className="w-full py-4">
        <div className="flex items-center justify-between px-8">
          <img className="block h-8 w-auto" src="/logo.svg" alt="CollAction" />
          <div className="">
            <div className="flex items-center">
              <span className="px-4 py-1 text-sm text-collaction cursor-default font-semibold rounded-full border border-gray-300">
                Coming soon
              </span>
            </div>
          </div>
        </div>
      </header>

      <section className="p-5 md:p-10 bg-black-0">
        <div className="container mx-auto grid grid-cols-1 md:grid-cols-2">
          <div className="p-2">
            <h1 className="text-collaction font-medium">Power to the Crowd</h1>
            <p>
              Do you want to make the world a better place, but do your actions
              feel like a drop in the ocean? Thanks to crowdacting you can take
              action, together with many like minded individuals. Let’s not eat
              meat for a month together. Let’s reduce plastic together.
              Together, we make waves!
            </p>
          </div>
          <div className="p-2">
            <h1 className="text-collaction font-medium">Our mission</h1>
            <p>
              Solve collective action problems together by digitally mobilizing
              your individual efforts and making it fun!
            </p>

            <h1 className="text-collaction pt-2 font-medium">Our vision</h1>
            <p>Solve all crowdacting problems in the world.</p>
          </div>
        </div>
      </section>

      <section className="p-5 py-10 md:p-10 text-center container mx-auto">
        <div className="text-center">
          <h1 className="text-collaction text-2xl font-medium">Time to act</h1>
          <span>CollAction stands for Collective Action. How to act?</span>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-x-5 mt-5 py-5 md:py-10 container">
          <div className="md:pt-2 md:pt-0">
            <img className="block h-20 w-auto mx-auto my-4" src="/steps/proposal.svg" />
            <h2 className="text-collaction text-lg">Goal</h2>
            Choose or suggest a goal you would like to participate in
          </div>
          <div className="pt-4 md:pt-2 md:pt-0">
            <img className="block h-20 w-auto mx-auto my-4" src="/steps/crowd.svg" />
            <h2 className="text-collaction text-lg">Crowd</h2>
            See how your actions are magnified by the crowd that has equal goals
          </div>
          <div className="pt-4 md:pt-2 md:pt-0">
            <img className="block h-20 w-auto mx-auto my-4" src="/steps/act.svg" />
            <h2 className="text-collaction text-lg">Action</h2>
            Commit to the goal and make impact
          </div>
        </div>
      </section>

      <section className="p-10 text-center bg-black-0">
        <div className="container mx-auto">
          <h1 className="text-collaction text-center text-2xl pb-4 font-medium">
            Who supports CollAction?
          </h1>
          <p>
            We couldn’t do what we’re doing without the support of our
            community, and generous donors:
          </p>
          <div className="flex py-6">
            <div className="bg-white px-6 py-5 max-w-full mx-auto w-72 border border-indigo-500 border-opacity-25 rounded-lg select-none overflow-hidden space-y-1">
              Henk Tetteroo
            </div>
          </div>
          <p>
            Would you like to support CollAction? <br /> Please send us a message at{" "}
            <a className="text-collaction" href="mailto:hello@collaction.org">
              hello@collaction.org
            </a>
          </p>
        </div>
      </section>

      <section className="px-5 py-10 md:px-10 md:py-20">
        <div className="container mx-auto">
          <div className="px-4">
            <h1 className="text-collaction text-2xl">
              What we are currently doing
            </h1>
            <p className="text-justify">
              Together with a team of developers from all over the world we’re
              building an app for iOS and Android. We aim to test the alpha
              version before October 2021. Would you like to receive updates on
              our progress and be one of the first to use our app? Join our
              mailing list here and/or follow us on Instagram.
            </p>
          </div>
          <div className="px-4 pt-6">
            <h1 className="text-collaction text-2xl">History</h1>
            <p className="text-justify">
              CollAction was founded in 2015 by Ron van den Akker and Spencer
              Heijnen. After great successes with fashion-related crowdactions,
              their team decided to focus solely on SlowFashion in 2020. As
              there is still strong confidence that there's room for CollAction
              in this world, a new team has formed in 2021 and is expanding
              rapidly.
            </p>
          </div>
        </div>
      </section>

      <section className="p-7 py-20 md:py-40 text-center bg-black-0">
        <div className="container mx-auto">
          <h1 className="text-collaction text-2xl font-bold">Want to help us out?</h1>
          <p>
            CollAction is run by volunteers only. Do you have a valuable skill
            set and want to help us out? <br /> Please send a message to{" "}
            <a className="text-collaction" href="mailto:hello@collaction.org">
              hello@collaction.org
            </a>
            .
          </p>
        </div>
      </section>

      <Footer />
    </div>
  );
};

export default Home;
