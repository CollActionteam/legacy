import type { NextPage } from "next";

import Team from "../components/home/Team";
import Ticker from "../components/home/Ticker";
import HelpOut from "../components/home/HelpOut"

const Home: NextPage = () => {
  return (
    <>
      {/* <Ticker /> */}
      <section className="p-5 py-10 md:p-10 text-center bg-black-0">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-x-5 pb-10 container mx-auto">
          <div className="pb-10 md:pb-0">
            <img
              className="block h-20 w-auto mx-auto my-4"
              src="/steps/proposal.svg"
            />
            <h2 className="text-collaction text-lg">Goal</h2>
            Choose or suggest a goal you would like to participate in
          </div>
          <div className="pb-10 md:pb-0">
            <img
              className="block h-20 w-auto mx-auto my-4"
              src="/steps/crowd.svg"
            />
            <h2 className="text-collaction text-lg">Crowd</h2>
            See how your actions are magnified by the crowd that has equal goals
          </div>
          <div className="pb-10 md:pb-0">
            <img
              className="block h-20 w-auto mx-auto my-4"
              src="/steps/act.svg"
            />
            <h2 className="text-collaction text-lg">Action</h2>
            Commit to the goal and make impact
          </div>
        </div>
        <div className="container mx-auto text-justify">
          <h1 className="text-collaction text-center text-2xl font-medium">
            What we do
          </h1>
          <span>
            We are CollAction. We will show that solving big Collective Action
            Problems like climate change is possible. Moreover, we will show
            that helping to solve these problems can be fun, and way easier than
            people think. We will show that action can be taken by your group of
            friends, your department of colleagues, or just by you together with
            strangers. You can make a real impact, make a real change, and have
            fun doing it. <br /> <br />
            On our app you can sign up for a challenge to participate in. We
            connect you to the people that also do the challenge, we show you
            what you have achieved, and we try our best to ensure everyone acts
            as promised. By doing so, we combine the power of the internet with
            the demand of people that want to take action, e.g. against climate
            change. <br /> <br />
            People want to act, but see that governments are moving too slow and
            that individual action is too small. By offering a hub for people to
            unite, we amplify individual impact. At the same time this hub keeps
            the barrier to act low and the impact immediate. Alone we are a drop
            in the ocean, together we make waves.
          </span>
        </div>
      </section>

      <section className="p-5 md:p-10 text-center">
        <div className="container mx-auto">
          <div className="pb-8">
            <h1 className="text-collaction text-2xl font-medium">
              Power to the Crowd
            </h1>
            <p>
              Do you want to make the world a better place, but do your actions
              feel like a drop in the ocean? Thanks to crowdacting you can take
              action, together with many like minded individuals. Let’s not eat
              meat for a month together. Let’s reduce plastic together.
              Together, we make waves!
            </p>
          </div>

          <div className="pb-8">
            <h1 className="text-collaction text-2xl font-medium">
              Our mission
            </h1>
            <p>Make doing good fun and easy!</p>
          </div>

          <div className="pb-8">
            <h1 className="text-collaction text-2xl font-medium">Our vision</h1>
            <p>
              Solve all{" "}
              <a
                href="https://en.wikipedia.org/wiki/Collective_action_problem"
                className="text-collaction"
                target="_blank"
                rel="noreferrer"
              >
                {" "}
                collective action problems
              </a>{" "}
              in the world.
            </p>
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
            Would you like to support CollAction? <br /> Please send us a
            message at{" "}
            <a className="text-collaction" href="mailto:hello@collaction.org">
              hello@collaction.org
            </a>
            .
          </p>
        </div>
      </section>

      <section className="px-5 py-10 md:px-10 md:py-20 text-center">
        <div className="container mx-auto">
          <div className="px-4">
            <h1 className="text-collaction text-2xl font-medium">
              What we are currently doing
            </h1>
            <p>
              Together with a team of developers from all over the world we’re
              building an app for iOS and Android. We aim to test the alpha
              version before October 2021. Would you like to receive updates on
              our progress and be one of the first to use our app? <br />
              <br />
              Join our{" "}
              <a
                className="text-collaction"
                target="_blank"
                rel="noreferrer"
                href="https://forms.gle/A233e3PHzA8VwEBGA"
              >
                mailing list
              </a>{" "}
              here and/or follow us on{" "}
              <a
                className="text-collaction"
                target="_blank"
                rel="noreferrer"
                href="https://www.instagram.com/collaction_org/"
              >
                Instagram
              </a>
              !
            </p>
          </div>
          <div className="px-4 pt-12">
            <h1 className="text-collaction text-2xl font-medium">History</h1>
            <p>
              CollAction was founded in 2015 by Ron van den Akker and Spencer
              Heijnen. After great successes with fashion-related crowdactions,
              their team decided to focus solely on{" "}
              <a className="text-collaction" href="https://slowfashion.global/">
                SlowFashion
              </a>{" "}
              in 2020. As there is still strong confidence that there&apos;s
              room for CollAction in this world, a new team has formed in 2021
              and is expanding rapidly.
            </p>
          </div>
        </div>
      </section>

      <Team name="new" title="Meet the team" background="bg-black-0" />
      <HelpOut />

      <Team
        name="old"
        title="The Giants"
        background="bg-black-0"
        description={
          <>
            You know the saying with shoulders and giants? <br />
            Without the team below, CollAction wouldn’t exist.
          </>
        }
      />
    </>
  );
};

export default Home;
