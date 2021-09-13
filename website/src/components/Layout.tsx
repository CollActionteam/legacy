import React, {ReactNode} from "react";
import Footer from "./Footer";

import NavigationBar from "./NavigationBar";

interface LayoutProps {
    children: ReactNode;
}

export default function Layout(props: LayoutProps) {
  return (
    <div className="bg-white text-black-400">
      <NavigationBar />
      {props.children}
      <Footer />
    </div>
  );
}
