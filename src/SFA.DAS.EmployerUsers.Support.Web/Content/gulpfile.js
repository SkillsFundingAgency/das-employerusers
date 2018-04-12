var gulp = require('gulp');
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');
var autoprefixer = require('gulp-autoprefixer');
var nunjucks = require('gulp-nunjucks');
var nunjucksRender = require('gulp-nunjucks-render');
var jshint = require('gulp-jshint');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');

var input = ['src/styles/*.scss'];
var output = 'dist/css/';

var sassOptions = {
    errLogToConsole: true,
    outputStyle: 'expanded',
    includePaths: [
        'node_modules/susy/sass',
        'node_modules/breakpoint-sass/stylesheets',
        'node_modules/normalize-scss/sass'
    ]
};

var autoprefixerOptions = {
    browsers: ['last 2 versions', '> 5%', 'Firefox ESR']
};

gulp.task('sass',
    function() {
        return gulp
            .src(input)
            .pipe(sass(sassOptions))
            .pipe(autoprefixer(autoprefixerOptions))
            .pipe(gulp.dest(output));
    });

gulp.task('build-js',
    function() {
        return gulp.src('src/scripts/*.js')
            .pipe(jshint())
            .pipe(jshint.reporter('jshint-stylish'))
            .pipe(concat('app.min.js'))
            .pipe(uglify())
            .pipe(gulp.dest('dist/js'));
    });

gulp.task('watch',
    function() {
        gulp.watch('src/scripts/*.js', ['build-js']);
        gulp.watch(['src/html/*.html', 'src/html/includes/*.html'], ['html']);
        gulp.watch(input, ['sass'])
            .on('change',
                function(event) {
                    console.log(`File ${event.path} was ${event.type}, running tasks...`);
                });
    });


gulp.task('html',
    function() {
        return gulp.src('src/html/*.html')
            .pipe(nunjucksRender({
                path: ['src/html/'] // String or Array 
            }))
            .pipe(gulp.dest('dist'));
    });

gulp.task('default', ['sass', 'build-js', 'watch']);