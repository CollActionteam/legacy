import React from "react";

import Image from "next/image";

interface AvatarProps {
  src: string;
  alt: string;
}

export default function Avatar(props: AvatarProps) {
  return (
    <div className="w-20 hflex flex-col text-center pb-4">
      <div className="block h-20">
        <Image
          src={props.src}
          className="rounded-full"
          alt={props.alt}
          width="100%"
          height="100%"
        />
      </div>
      <p className="pt-2">{props.alt}</p>
    </div>
  );
}
