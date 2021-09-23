import React from "react";

export default function Vision() {
  return (
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
            meat for a month together. Let’s reduce plastic together. Together,
            we make waves!
          </p>
        </div>

        <div className="pb-8">
          <h1 className="text-collaction text-2xl font-medium">Our mission</h1>
          <p>Make doing good fun and easy!</p>
        </div>

        <div>
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
  );
}
