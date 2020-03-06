import React from "react";
// import styles from "./Overlay.module.scss";

interface IOverlayProps {
  children: any;
  photo: string;
}

export const Overlay = ({ children }: IOverlayProps) => {
  // const whiteOverlay = "rgba(256, 256, 256, 0.8)";
  // const overlay = `linear-gradient(${whiteOverlay}, ${whiteOverlay})`;

  return <div>{children}</div>;
};