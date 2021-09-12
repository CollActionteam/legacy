import type { NextPage } from "next";

import Footer from "../components/Footer";
import Team from "../components/Team";
import NavigationBar from "../components/NavigationBar";

const Home: NextPage = () => {
  return (
    <div className="bg-white text-black-400">
      <NavigationBar />
      <section className="p-5 md:p-10 bg-black-0 text-center">
        <div className="container mx-auto">
          <div className="pb-8">
            <h1 className="text-collaction font-medium">Power to the Crowd</h1>
            <p>
              Do you want to make the world a better place, but do your actions
              feel like a drop in the ocean? Thanks to crowdacting you can take
              action, together with many like minded individuals. Let’s not eat
              meat for a month together. Let’s reduce plastic together.
              Together, we make waves!
            </p>
          </div>

          <div className="pb-8">
            <h1 className="text-collaction font-medium">Our mission</h1>
            <p>Make doing good fun and easy!</p>
          </div>

          <div className="pb-8">
            <h1 className="text-collaction pt-2 font-medium">Our vision</h1>
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
      <section className="p-5 py-10 md:p-10 text-center container mx-auto">
        <div className="text-center">
          <h1 className="text-collaction text-2xl font-medium">Time to act</h1>
          <span>CollAction stands for Collective Action. How to act?</span>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-x-5 py-5 md:py-5 container">
          <div>
            <img
              className="block h-20 w-auto mx-auto my-4"
              src="/steps/proposal.svg"
            />
            <h2 className="text-collaction text-lg">Goal</h2>
            Choose or suggest a goal you would like to participate in
          </div>
          <div>
            <img
              className="block h-20 w-auto mx-auto my-4"
              src="/steps/crowd.svg"
            />
            <h2 className="text-collaction text-lg">Crowd</h2>
            See how your actions are magnified by the crowd that has equal goals
          </div>
          <div>
            <img
              className="block h-20 w-auto mx-auto my-4"
              src="/steps/act.svg"
            />
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
              their team decided to focus solely on SlowFashion in 2020. As
              there is still strong confidence that there&apos;s room for
              CollAction in this world, a new team has formed in 2021 and is
              expanding rapidly.
            </p>
          </div>
        </div>
      </section>
      <Team name="new" title="Meet the team" background="bg-black-0" />
      <Team
        name="old"
        title="The Giants"
        description={
          <>
            You know the saying with shoulders and giants? <br />
            Without the team below, CollAction would not exist.
          </>
        }
      />
      <section className="p-7 py-20 text-center">
        <div className="container mx-auto">
          <h1 className="text-collaction text-2xl font-medium">
            Want to help us out?
          </h1>
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
