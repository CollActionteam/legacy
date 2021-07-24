import { LazyLoadImageProps, LazyLoadImage } from "react-lazy-load-image-component";
import React from "react";
import styles from "./LazyImage.module.scss";

const LazyImage = (props: LazyLoadImageProps) => {
    return <span className={styles.lazyImage}>
        <LazyLoadImage {...props} />
    </span>;
};

export default LazyImage;