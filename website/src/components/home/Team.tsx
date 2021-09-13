import React, { ReactNode } from "react";
import clsx from "clsx";

import Avatar from "../Avatar";
import new_team from "../../data/new_team.json";
import old_team from "../../data/old_team.json";

interface TeamProps {
  name: string | "new" | "old";
  title: string;
  description?: string | ReactNode;
  background?: string;
}

interface IMember {
  name: string;
  full_name?: string;
  photo: string;
  linkedin_github?: string;
}

export default function Teams(props: TeamProps) {
  var team_members = new_team.members.map((member: IMember) => {
    let prefix = "/teams/" + props.name + "/";
    return (
      <Avatar
        key={member.name}
        src={prefix + member.photo}
        alt={member.name}
        linkedin_github={member.linkedin_github}
      />
    );
  });

  if (props.name === "old") {
    team_members = old_team.members.map((member: IMember) => {
      let prefix = "/teams/" + props.name + "/";
      return (
        <Avatar key={member.name} src={prefix + member.photo} alt={member.name} />
      );
    });
  }

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
          {team_members}
        </div>
      </div>
    </section>
  );
}
