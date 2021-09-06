import React, { ReactElement } from "react";

import { InstagramLogo } from 'phosphor-react';

export default function Footer(): ReactElement {
  // Contact details
  // Social Media logos with hyperlink
  // kvk information

  return (
    <div className="flex pb-5 px-3 m-auto pt-5 text-gray-800 text-sm flex-col md:flex-row max-w-6xl">
      <div className="mt-2 text-center md:text-left">© Copyright 2021. All Rights Reserved.</div>
      <div className="md:flex-auto md:flex-row-reverse mt-2 flex-row flex justify-center md:justify-start">
        <a href="https://www.instagram.com/collaction_org/" target="_blank" className="w-6 mx-1">
          <InstagramLogo size={24} />
        </a>
      </div>
    </div>
  );
}
