import React from "react";
import styles from "./Ovelray.module.scss";

interface IOverlayProps {
  children: any;
  photo: string;
}

export const Overlay = ({ children, photo }: IOverlayProps) => {
  const whiteOverlay = "rgba(256, 256, 256, 0.8)";
  const overlay = `linear-gradient(${whiteOverlay}, ${whiteOverlay})`;

    return <div>{children}</div>;
};