import React, { ReactNode } from "react";
import clsx from "clsx";

import Avatar from "../Avatar";
import people from "../../lib/people.json";

interface TeamProps {
  name: "new" | "old";
  title: string;
  description?: string | ReactNode;
  background?: string;
}

interface IMember {
  name: string;
  photo: string;
  linkedin_github?: string;
}

export default function Teams(props: TeamProps) {
  let teams = {
    ...people,
    old: [
      {
        name: "Arnout",
        photo: "nophoto.png",
      },
      {
        name: "Clara",
        photo: "clara-doyle.jpg",
      },
      {
        name: "Daniela",
        photo: "daniela-becker.png",
      },
      {
        name: "Dominik",
        photo: "nophoto.png",
      },
      {
        name: "Edoardo",
        photo: "edoardo-felici.png",
      },
      {
        name: "Georgie",
        photo: "georgie-kusbiantoro.png",
      },
      {
        name: "Jeppe",
        photo: "jeppe-bijker.jpg",
      },
      {
        name: "Jonas",
        photo: "jonas-s.jpg",
      },
      {
        name: "Kisjonah",
        photo: "kisjonah-roos.jpg",
      },
      {
        name: "Lena",
        photo: "lena-hartog.jpg",
      },
      {
        name: "Luuk",
        photo: "luuk-boschker.png",
      },
      {
        name: "Laura",
        photo: "laura-c.jpeg",
      },
      {
        name: "Lucie",
        photo: "lucie-morauw.jpeg",
      },
      {
        name: "Mara",
        photo: "mara-de-pater.jpg",
      },
      {
        name: "Ron",
        photo: "ron-van-den-akker.png",
      },
      {
        name: "Shirley",
        photo: "shirley-zheng.jpeg",
      },
      {
        name: "Spencer",
        photo: "spencer-heijnen.png",
      },
      {
        name: "Tim",
        photo: "tim-stokman.png",
      },
      {
        name: "Tuanh",
        photo: "tuanh-lam.jpg",
      },
    ],
  };

  return (
    <section className={clsx("px-5", "py-10", props.background || "")}>
      <div
        className={clsx(
          "pb-10",
          "pb-12" && props.name === "old",
          "text-center"
        )}
      >
        <h1
          className={clsx(
            "text-collaction",
            "text-2xl",
            "font-medium",
            props.description && "mb-4"
          )}
        >
          {props.title}
        </h1>
        {props.description && <p>{props.description}</p>}
      </div>
      <div className="container mx-auto">
        <div className="grid grid-cols-3 md:grid-cols-4 justify-items-center gap-px mx-auto max-w-md">
          {teams[props.name].map((member: IMember) => {
            let prefix = "/teams/" + props.name + "/";
            return (
              <Avatar
                key={member.name}
                src={prefix + member.photo}
                alt={member.name}
                linkedin_github={member.linkedin_github}
              />
            );
          })}
        </div>
      </div>
    </section>
  );
}
