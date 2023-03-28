"use strict";

const gulp = require("gulp"),
    rename = require('gulp-rename'),
    fs = require('fs');

//todo: postinstall

function copyPackageJsToWwwroot(packageName, srcFilename) {
    // Read the package.json file to get the package version
    const packageJson = JSON.parse(fs.readFileSync(`../${packageName}/package.json`));
    const packageVersion = packageJson.version;

    // Set the destination file name
    const destFileName = `${packageName}-${packageVersion}.min.js`;

    // Copy and rename the file
    return gulp.src(`../${packageName}/${srcFilename}`)
        .pipe(rename(destFileName))
        .pipe(gulp.dest('../../wwwroot/js'));
}

/*todo: min version is not in package, do we min it ourselves, or have manual step to add it, or add the min version into our package and copy it <--- this one*/
/*todo: copy the map files too*/
/*todo: copy the ie8 css files too*/
gulp.task('copy-govuk-frontend-js', function () {
    return copyPackageJsToWwwroot('govuk-frontend', 'govuk/all.js');
});

gulp.task('copy-familyhubs-frontend-js', function () {
    return copyPackageJsToWwwroot('familyhubs-frontend', 'all.min.js');
});

gulp.task('copy-packages-js', gulp.series('copy-govuk-frontend-js', 'copy-familyhubs-frontend-js'));

//todo: delegate from consumer gulp to this gulp?