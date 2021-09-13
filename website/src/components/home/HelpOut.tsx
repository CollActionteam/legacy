import React from "react";

export default function HelpOut() {
  return (
    <section className="p-7 py-20 text-center">
      <div className="container mx-auto">
        <h1 className="text-collaction text-2xl font-medium">
          Want to help us out?
        </h1>
        <p>
          CollAction is run by volunteers only. Do you have a valuable skill set
          and want to help us out? <br /> Please send a message to{" "}
          <a className="text-collaction" href="mailto:hello@collaction.org">
            hello@collaction.org
          </a>
          .
        </p>
      </div>
    </section>
  );
}
