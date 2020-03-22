// Go to https://kims-first-project-c74994-026f210495460.webflow.io/
// Go to the Chrome Developer Tools
// Go to Sources > Snippets > create a new snippet and paste this entire file
// into it.
// Run it.
// Wait a few seconds for it to finish.
// Paste the result into a CSS optimizer.
// For example -
// https://www.cssportal.com/css-optimize/ and choose "Highest" compression
// For Regroup selectors, choose to merge selectors with the same properties
// For Case for properties, select Lowercase
// Tick only the following -
// Compress colors
// Compress font-weight
// Lowercase selectors
// Remove unnecessary backslashes
// Remove last ;
copy = copy;
(async function() {
  frames1 = typeof frames1 === "undefined" ? [] : frames1;
  images = [...document.querySelector(".section-2").children];
  imageCount = images.length
  function delay(time=5) {
    return new Promise((resolve)=>setTimeout(resolve, time));
  }
  window.scrollTo(0, 0);
  await delay(500);
  if (!frames1.length)
    for (i = 0; i < 1276; i++) {
      window.scrollTo(0, i);
      await delay();
      frames1.push(images.map(image=>({
        opacity: parseFloat(parseFloat(getComputedStyle(image).opacity).toFixed(2)),
        top: image.getClientRects()[0].top | 0,
        left: image.getClientRects()[0].left | 0
      })));
    }
  minimums = frames1.reduce((result,frame)=>frame.reduce((result1,item)=>{
    result1.top = Math.min(item.top, result1.top);
    result1.left = Math.min(item.left, result1.left);
    return result1
  }
  , result), {
    top: 10000,
    left: 10000
  });
  relativeFrames = frames1.map(frame=>frame.map(item=>({
    opacity: item.opacity,
    top: item.top - minimums.top,
    left: item.left - minimums.left
  })));
  skippedStillFrames = [];
  optimizedFrames = relativeFrames.reduce((result,frame,index)=>{
    if (!index || JSON.stringify(frame) !== JSON.stringify(result[result.length - 1].frame)) {
      result.push({
        frame: frame,
        index
      });
      if (index && (index - result[result.length - 2].index) > 8) {
        skippedStillFrames.push({
          index: result.length - 1,
          duration: index - result[result.length - 2].index
        });
      }
    }
    return result;
  }
  , []);
  stringifiedTranslates = optimizedFrames.map(({frame})=>frame.map(item=>`${item.left},${item.top}`));
  translateCombinations = (new Array(imageCount).join('\n')).split('\n').map(()=>[]);
  translateCombinationCounts = (new Array(imageCount).join('\n')).split('\n').map(()=>[]);
  ;stringifiedTranslates.forEach(frame=>frame.forEach((item,index)=>{
    let combinationIndex = translateCombinations[index].indexOf(item);
    if (combinationIndex === -1) {
      combinationIndex = translateCombinations[index].push(item) - 1;
      translateCombinationCounts[index].push(0);
    }
    translateCombinationCounts[index][combinationIndex]++;
  }
  ));
  stringifiedOpacity = optimizedFrames.map(({frame})=>frame.map(item=>item.opacity));
  opacityCombinations = (new Array(imageCount).join('\n')).split('\n').map(()=>[]);
  opacityCombinationCounts = (new Array(imageCount).join('\n')).split('\n').map(()=>[]);
  stringifiedOpacity.forEach(frame=>frame.forEach((item,index)=>{
    let combinationIndex = opacityCombinations[index].indexOf(item);
    if (combinationIndex === -1) {
      combinationIndex = opacityCombinations[index].push(item) - 1;
      opacityCombinationCounts[index].push(0);
    }
    opacityCombinationCounts[index][combinationIndex]++;
  }
  ));

  dominantOpacities = opacityCombinations.map((imageOpacities,index)=>imageOpacities[opacityCombinationCounts[index].indexOf(Math.max(...opacityCombinationCounts[index]))])
  dominantTranslations = translateCombinations.map((imageTranslates,index)=>imageTranslates[translateCombinationCounts[index].indexOf(Math.max(...translateCombinationCounts[index]))])
  
  dominantOpacitiesCSS = 
  dominantOpacities.map((opacity,index)=>`.b${index} {\n    opacity: ${opacity};\n}`);
  dominantTranslationsCSS = 
  dominantTranslations.map((translation,index)=> `.b${index} {\n    transform: translate(${translation.split(",").join("px, ")}px);\n}`).filter(Boolean);

  css = dominantOpacitiesCSS.concat(dominantTranslationsCSS).concat(optimizedFrames.map((frame,index)=>frame.frame.map((item,itemIndex)=>{
    let [left, top] = dominantTranslations[itemIndex].split(",");
    translation = item.opacity !== 0 && (parseInt(top, 10) !== item.top || parseInt(left, 10) !== item.left)? `\n  transform: translate(${item.left}px, ${item.top}px);` : "";
    opacity = item.opacity !== dominantOpacities[itemIndex] ? `\n  opacity: ${item.opacity};` : '';
    if (!translation && !opacity) {
      return '';
    } else {
      return `.f${index} .b${itemIndex} {${translation}${opacity}\n}`
    }
  }
  ).join('\n')));
  copy(css.join('\n'));
  allLists = [frames1, minimums, relativeFrames, optimizedFrames, skippedStillFrames, stringifiedTranslates, translateCombinations, translateCombinationCounts, stringifiedOpacity, opacityCombinations, opacityCombinationCounts, dominantOpacities, dominantTranslations, dominantOpacitiesCSS, dominantTranslationsCSS, css];
  console.log(allLists);
}());
