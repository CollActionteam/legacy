import React from "react";

export default function Supporters() {
  return (
    <section className="p-10 text-center bg-black-0">
      <div className="container mx-auto">
        <h1 className="text-collaction text-center text-2xl pb-4 font-medium">
          Who supports CollAction?
        </h1>
        <p>
          We couldn’t do what we’re doing without the support of our community,
          and generous donors:
        </p>
        <div className="flex py-6">
          <div className="bg-white px-6 py-5 max-w-full mx-auto w-72 border border-indigo-500 border-opacity-25 rounded-lg select-none overflow-hidden space-y-1">
            Henk Tetteroo
          </div>
        </div>
        <p>
          Would you like to support CollAction? <br /> Please send us a message
          at{" "}
          <a className="text-collaction" href="mailto:tom@collaction.org">
            tom@collaction.org
          </a>
          .
        </p>
      </div>
    </section>
  );
}
