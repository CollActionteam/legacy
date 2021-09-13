import React from "react";

interface AvatarProps {
  src: string;
  alt: string;
  linkedin_github?: string;
}

export default function Avatar(props: AvatarProps) {
  return (
    <div className="w-20 hflex flex-col text-center pb-4">
      <a href={props.linkedin_github}>
        <div className="block h-20">
          <img
            src={props.src}
            className="rounded-full"
            alt={props.alt}
            width="80px"
            height="80px"
          />
        </div>
        <p className="pt-2">{props.alt}</p>
      </a>
    </div>
  );
}
