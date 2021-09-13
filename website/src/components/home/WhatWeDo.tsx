import React from "react";

export default function WhatWeDo() {
  return (
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
          Problems like climate change is possible. Moreover, we will show that
          helping to solve these problems can be fun, and way easier than people
          think. We will show that action can be taken by your group of friends,
          your department of colleagues, or just by you together with strangers.
          You can make a real impact, make a real change, and have fun doing it.{" "}
          <br /> <br />
          On our app you can sign up for a challenge to participate in. We
          connect you to the people that also do the challenge, we show you what
          you have achieved, and we try our best to ensure everyone acts as
          promised. By doing so, we combine the power of the internet with the
          demand of people that want to take action, e.g. against climate
          change. <br /> <br />
          People want to act, but see that governments are moving too slow and
          that individual action is too small. By offering a hub for people to
          unite, we amplify individual impact. At the same time this hub keeps
          the barrier to act low and the impact immediate. Alone we are a drop
          in the ocean, together we make waves.
        </span>
      </div>
    </section>
  );
}
